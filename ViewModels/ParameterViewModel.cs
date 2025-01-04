using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using SofyTrender.Models;
using SofyTrender.Pages;
using SofyTrender.Services;
using SpotifyAPI.Web;

namespace SofyTrender.ViewModels
{
    public partial class ParameterViewModel : ObservableObject
    {
        const int AllowedSeeds = 5;

        public event Action PlaylistChanged;

        public ICommand GenerateNewCommand { get; set; }
        public ICommand RegenerateCommand { get; set; }
        public ICommand AddGenreCommand { get; set; }
        public ICommand AddArtistCommand { get; set; }
        public ICommand AddTrackCommand { get; set; }
        public ICommand RemoveGenreCommand { get; set; }
        public ICommand RemoveArtistCommand { get; set; }
        public ICommand RemoveTrackCommand { get; set; }
        public ICommand RemovePlaylistCommand { get; set; }
        public ICommand PopularityChangedCommand { get; set; }
        public ICommand NameChangedCommand { get; set; }

        public ObservableCollection<string> Genres { get; set; } = [];
        public ObservableCollection<FullArtist> Artists { get; set; } = [];
        public ObservableCollection<FullTrack> Tracks { get; set; } = [];
        public ObservableCollection<FullPlaylist> GeneratedPlaylists { get; set; } = [];

        [ObservableProperty] string name;
        [ObservableProperty] int seedCount;
        [ObservableProperty] double minYear;
        [ObservableProperty] double maxYear;
        [ObservableProperty] bool useYears;
        [ObservableProperty] bool usePopularity;
        [ObservableProperty] double popularity;

        Window? _searchWindow;
        PlaylistSaveData _playListData;

#pragma warning disable CS8618
        public ParameterViewModel(PlaylistSaveData playlist)
        {
            GenerateNewCommand = new Command(Generate, HasAnySeeds);
            RegenerateCommand = new Command(Regenerate, _ => HasAnySeeds());
            AddGenreCommand = new Command(AddGenre, HasFreeSeeds);
            AddArtistCommand = new Command(AddArtist, HasFreeSeeds);
            AddTrackCommand = new Command(AddTrack, HasFreeSeeds);
            RemoveGenreCommand = new Command(RemoveGenre);
            RemoveArtistCommand = new Command(RemoveArtist);
            RemoveTrackCommand = new Command(RemoveTrack);
            RemovePlaylistCommand = new Command(RemovePlaylist);
            PopularityChangedCommand = new Command(PopularityChanged);
            NameChangedCommand = new Command(NameChanged);

            _playListData = playlist;
            Name = _playListData.name;
            var (min, max) = Utils.ConvertFromYears(_playListData.minYear, _playListData.maxYear);
            MinYear = min;
            MaxYear = max;
            UseYears = _playListData.useYears;
            UsePopularity = _playListData.usePopularity;
            Popularity = _playListData.popularity / 100d;

            RefreshCollections();
        }

        bool HasFreeSeeds()
        {
            return SeedCount < AllowedSeeds;
        }

        bool HasAnySeeds()
        {
            return SeedCount > 0;
        }

        void ChangeSeedCount(int value)
        {
            SeedCount += value;
            ((Command)AddGenreCommand).ChangeCanExecute();
            ((Command)AddArtistCommand).ChangeCanExecute();
            ((Command)AddTrackCommand).ChangeCanExecute();
            ((Command)GenerateNewCommand).ChangeCanExecute();
        }

        void RefreshCollections()
        {
            foreach (var genre in _playListData.genres)
            {
                Genres.Add(genre);
                ChangeSeedCount(1);
            }

            foreach (var artist in _playListData.artists)
            {
                Artists.Add(artist);
                ChangeSeedCount(1);
            }

            foreach (var track in _playListData.tracks)
            {
                Tracks.Add(track);
                ChangeSeedCount(1);
            }

            foreach (var playlist in _playListData.generatedPlaylists)
            {
                GeneratedPlaylists.Add(playlist);
            }
        }

        async void Generate()
        {
            var recommendationsRequest = UseYears ? await GetRecommendationsByYears() : GetSimpleRecommendations();
            if (UsePopularity) recommendationsRequest.Target.Add("popularity", "" + (int)(Popularity * 100));

            var response = await SpotifyService.SpotifyClient?.Browse.GetRecommendations(recommendationsRequest) ?? null;
            if (response != null && response.Tracks.Count == 0)
            {
                Application.Current?.MainPage.DisplayAlert("Error", $"Something went wrong, we could not generate enough Tracks", "sad...");
                return;
            }

            var currentUser = await SpotifyService.SpotifyClient.UserProfile.Current();
            var createPlaylist = new PlaylistCreateRequest(Name);
            createPlaylist.Public = false;

            var createdPlaylist = await SpotifyService.SpotifyClient.Playlists.Create(currentUser.Id, createPlaylist);
            var addRequest = new PlaylistAddItemsRequest(response.Tracks.Select(s => s.Uri).ToList());

            await SpotifyService.SpotifyClient.Playlists.AddItems(createdPlaylist.Id, addRequest);

            GeneratedPlaylists.Add(createdPlaylist);
            Save();

            Application.Current?.MainPage.DisplayAlert("Playlist", $"Created a new Playlist '{createdPlaylist.Name}' with several new Tracks! ", "Ok");
        }

        async void Regenerate(object playlist)
        {
            var recommendationsRequest = UseYears ? await GetRecommendationsByYears() : GetSimpleRecommendations();
            var response = await SpotifyService.SpotifyClient.Browse.GetRecommendations(recommendationsRequest);
            if (UsePopularity) recommendationsRequest.Target.Add("popularity", "" + (int)(Popularity * 100));

            if (response.Tracks.Count == 0)
            {
                Application.Current?.MainPage.DisplayAlert("Error", $"Something went wrong, we could not generate enough Tracks", "sad...");
                return;
            }

            FullPlaylist savedPlaylist = (FullPlaylist)playlist;

            //Check for an updated Playlist
            var createdPlaylist = await SpotifyService.SpotifyClient.Playlists.Get(savedPlaylist.Id);
            if (createdPlaylist.Tracks.Items.Count > 0)
            {
                var request = new PlaylistRemoveItemsRequest();
                request.Tracks = createdPlaylist.Tracks.Items
                    .Select(i => new PlaylistRemoveItemsRequest.Item() { Uri = ((FullTrack)i.Track).Uri })
                    .ToList();

                await SpotifyService.SpotifyClient.Playlists.RemoveItems(createdPlaylist.Id, request);
            }

            var addRequest = new PlaylistAddItemsRequest(response.Tracks.Select(s => s.Uri).ToList());

            await SpotifyService.SpotifyClient.Playlists.AddItems(createdPlaylist.Id, addRequest);

            Application.Current?.MainPage.DisplayAlert("Playlist", $"Regenerated new Tracks for Playlist '{createdPlaylist.Name}'! ", "Cool!");
        }

        RecommendationsRequest GetSimpleRecommendations()
        {
            var recommendationsRequest = new RecommendationsRequest();
            foreach (var genre in _playListData.genres)
            {
                recommendationsRequest.SeedGenres.Add(genre);
            }

            foreach (var artist in _playListData.artists)
            {
                recommendationsRequest.SeedArtists.Add(artist.Id);
            }

            foreach (var track in _playListData.tracks)
            {
                recommendationsRequest.SeedTracks.Add(track.Id);
            }

            return recommendationsRequest;
        }

        async Task<RecommendationsRequest> GetRecommendationsByYears()
        {
            var recommendationsRequest = new RecommendationsRequest();
            var tracks = new List<FullTrack>();
            var allowedTracksPerSeed = SeedCount / AllowedSeeds;
            var rolledSeeds = 0;

            foreach (var track in _playListData.tracks)
            {
                tracks.Add(track);
                rolledSeeds++;
            }

            foreach (var genre in _playListData.genres)
            {
                await SearchAndAddTracks("genre:" + genre);
                rolledSeeds++;
            }

            await Task.Delay(1000);

            foreach (var artist in _playListData.artists)
            {
                await SearchAndAddTracks("artist:" + artist.Name);
                rolledSeeds++;
            }

            await Task.Delay(1000);

            var featuresRequest = new TracksAudioFeaturesRequest(tracks.Select(t => t.Id).ToList());
            var analysis = await SpotifyService.SpotifyClient.Tracks.GetSeveralAudioFeatures(featuresRequest);

            TrackAudioFeatures result = new TrackAudioFeatures();
            foreach (var trackFeature in analysis.AudioFeatures)
            {
                result.Loudness += trackFeature.Loudness / tracks.Count;
                result.Acousticness += trackFeature.Acousticness / tracks.Count;
                result.Danceability += trackFeature.Danceability / tracks.Count;
                result.Valence += trackFeature.Valence / tracks.Count;
                result.Instrumentalness += trackFeature.Instrumentalness / tracks.Count;
                result.Speechiness += trackFeature.Speechiness / tracks.Count;
                result.Tempo += trackFeature.Tempo / tracks.Count;
                result.Mode += trackFeature.Mode / tracks.Count;
                result.DurationMs += trackFeature.DurationMs / tracks.Count;
            }

            recommendationsRequest.Target.Add("loudness", "" + result.Loudness);
            recommendationsRequest.Target.Add("acousticness", "" + result.Acousticness);
            recommendationsRequest.Target.Add("danceability", "" + result.Danceability);
            recommendationsRequest.Target.Add("valence", "" + result.Valence);
            recommendationsRequest.Target.Add("instrumentalness", "" + result.Instrumentalness);
            recommendationsRequest.Target.Add("speechiness", "" + result.Speechiness);
            recommendationsRequest.Target.Add("tempo", "" + result.Tempo);
            recommendationsRequest.Target.Add("mode", "" + result.Mode);
            recommendationsRequest.Target.Add("duration_ms", "" + result.DurationMs);

            var artistsRequest = new ArtistsRequest(tracks.Select(t => t.Artists[0].Id).ToList());
            var artists = await SpotifyService.SpotifyClient.Artists.GetSeveral(artistsRequest);
            foreach(var ar in artists.Artists)
            {
                foreach(var gen in ar.Genres)
                {
                    Debug.WriteLine("Genre: " + gen);
                }
            }
            await Task.Delay(1000);

            var years = Utils.ConvertFromValues(MinYear, MaxYear);
            foreach (var genre in artists.Artists.Select(a => a.Genres[0]))
            {
                var searchQuery = "year:" + years.minYear + "-" + years.maxYear + " genre:" + genre;
                var searchRequest = new SearchRequest(SearchRequest.Types.Track, searchQuery);
                searchRequest.Limit = 20;
                var response = await SpotifyService.SpotifyClient.Search.Item(searchRequest);
                if (response.Tracks.Items.Count == 0)
                {
                    Application.Current?.MainPage.DisplayAlert("Error", $"Something went wrong, we could not generate enough Tracks (Query={searchQuery})", "sad...");
                    continue;
                }
                var index = (int)(Random.Shared.NextDouble() * response.Tracks.Items.Count);
                Debug.WriteLine("Get Song at index:" + index);
                recommendationsRequest.SeedTracks.Add(response.Tracks.Items[index].Id);
            }

            return recommendationsRequest;

            async Task SearchAndAddTracks(string query)
            {
                var searchRequest = new SearchRequest(SearchRequest.Types.Track, query);
                var response = await SpotifyService.SpotifyClient.Search.Item(searchRequest);
                var allowedTracks = RollAllowedTracks();
                Debug.WriteLine("AllowedTracks: " + allowedTracks);
                for (var i = 0; i < allowedTracks; i++)
                {
                    tracks.Add(response.Tracks.Items[(int)(Random.Shared.NextDouble() * response.Tracks.Items.Count)]);
                }
            }

            int RollAllowedTracks()
            {
                var freeTrackCount = AllowedSeeds - tracks.Count;
                float upcomingSeeds = SeedCount - rolledSeeds;

                var allowedValue = freeTrackCount / upcomingSeeds;
                int baseValue = (int)MathF.Floor(allowedValue);
                return baseValue + (allowedValue - baseValue > Random.Shared.NextDouble() ? 1 : 0);
            }

        }

        void AddGenre()
        {
            var viewModel = new SearchViewModel<string>(new GenreSearchModel());
            viewModel.ItemSelected += ItemSelected;

            var searchPage = new SearchPage();
            searchPage.SetViewModel(viewModel);
            ShowSearchpage(searchPage);

            void ItemSelected(object selectedItem)
            {
                var item = (string)selectedItem;
                Debug.WriteLine("Selected Genre: " + item);
                Genres.Add(item);
                ChangeSeedCount(1);
                CloseSearchWindow();
                Save();
            }
        }

        void AddArtist()
        {
            var viewModel = new SearchViewModel<FullArtist>(new ArtistSearchModel());
            viewModel.ItemSelected += ItemSelected;

            var searchPage = new SearchPage();
            searchPage.SetViewModel(viewModel);
            ShowSearchpage(searchPage);

            void ItemSelected(object selectedItem)
            {
                var item = (FullArtist)selectedItem;
                Debug.WriteLine("Selected Artist: " + item.Name + " id:" + item.Id);
                Artists.Add(item);
                ChangeSeedCount(1);
                CloseSearchWindow();
                Save();
            }
        }

        void AddTrack()
        {
            var viewModel = new SearchViewModel<FullTrack>(new TrackSearchModel());
            viewModel.ItemSelected += ItemSelected;

            var searchPage = new SearchPage();
            searchPage.SetViewModel(viewModel);
            ShowSearchpage(searchPage);

            void ItemSelected(object selectedItem)
            {
                var item = (FullTrack)selectedItem;
                Debug.WriteLine("Selected Artist: " + item.Name + " by " + item.Artists[0].Name + " id:" + item.Id);
                Tracks.Add(item);
                ChangeSeedCount(1);
                CloseSearchWindow();
                Save();
            }
        }

        void RemoveGenre(object selectedItem)
        {
            Genres.Remove((string)selectedItem);
            ChangeSeedCount(-1);
            Save();
        }

        void RemoveArtist(object selectedItem)
        {
            Artists.Remove((FullArtist)selectedItem);
            ChangeSeedCount(-1);
            Save();
        }

        void RemoveTrack(object selectedItem)
        {
            Tracks.Remove((FullTrack)selectedItem);
            ChangeSeedCount(-1);
            Save();
        }

        void RemovePlaylist(object selectedItem)
        {
            GeneratedPlaylists.Remove((FullPlaylist)selectedItem);
            Save();
        }

        void PopularityChanged()
        {
            Debug.WriteLine("Pop now:" + Popularity);
            Save();
        }

        void NameChanged()
        {
            Debug.WriteLine("Name now:" + name);
            Save();
        }

        void Save()
        {
            Debug.WriteLine("name: " + Name);
            _playListData.name = Name;
            _playListData.genres = Genres.ToArray();
            _playListData.artists = Artists.ToArray();
            _playListData.tracks = Tracks.ToArray();
            _playListData.generatedPlaylists = GeneratedPlaylists.ToArray();

            var years = Utils.ConvertFromValues(MinYear, MaxYear);
            _playListData.minYear = years.minYear;
            _playListData.maxYear = years.maxYear;

            _playListData.useYears = UseYears;

            _playListData.popularity = (int)(Popularity * 100);
            _playListData.usePopularity = UsePopularity;

            PlaylistChanged?.Invoke();
        }

        void ShowSearchpage(SearchPage searchPage)
        {
            searchPage.Parent = null;
            Application.Current?.MainPage?.Navigation.PushAsync(searchPage);
        }

        void CloseSearchWindow()
        {
            Application.Current?.MainPage?.Navigation.PopAsync();
            return;
        }
    }
}
