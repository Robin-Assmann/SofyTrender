using System.Collections.ObjectModel;
using System.Diagnostics;
using SofyTrender.Services;
using SpotifyAPI.Web;

namespace SofyTrender.Models
{
    internal class TrackSearchModel : ISearchModel<FullTrack>
    {
        public ObservableCollection<FullTrack> Items { get; } = new ObservableCollection<FullTrack>();

        public TrackSearchModel()
        {
        }

        public void Search(string input)
        {
            Debug.WriteLine("TrackSearchModel:Search " + input);
            DoSearch();

            async void DoSearch()
            {
                Items.Clear();
                var searchRequest = new SearchRequest(SearchRequest.Types.Track, input);
                var search = await SpotifyService.SpotifyClient.Search.Item(searchRequest);
                if (search.Tracks.Items != null)
                {
                    foreach (var track in search.Tracks.Items)
                    {
                        Items.Add(track);
                    }
                }
            }
        }

        public void TextChange(string input)
        {
            Debug.WriteLine("ArtistSearchModel:TextChange " + input);
        }
    }
}

