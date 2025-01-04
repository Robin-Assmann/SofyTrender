using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using SofyTrender.Custom;
using SofyTrender.Models;
using SofyTrender.Pages;
using SofyTrender.Services;

namespace SofyTrender.ViewModels
{
    partial class OverviewViewModel : ObservableObject
    {
        public event Action<PlaylistSaveData>? PlaylistSelected;

        public ICommand SelectPlaylistCommand { get; set; }
        public ICommand AddPlaylistCommand { get; set; }
        public ICommand RemovePlaylistCommand { get; set; }

        public string HeaderText { get; set; } = "Overview";
        public ObservableCollection<PlaylistSaveData> Playlists { get; private set; } = [];

        [ObservableProperty] object? _selectedItem;

        public OverviewViewModel()
        {
            SelectPlaylistCommand = new Command(SelectPlaylist);
            AddPlaylistCommand = new Command(AddPlaylist);
            RemovePlaylistCommand = new Command(RemovePlaylist);

            LoadPlaylists();
        }

        async void LoadPlaylists()
        {
            var result = await DataStoreService.LoadPlaylists();
            if (result == null) return;

            foreach (var playlist in result)
            {
                Playlists.Add(playlist);
            }
        }

        void AddPlaylist()
        {
            Playlists.Add(new PlaylistSaveData());
            SavePlaylists();
        }

        void RemovePlaylist(object playlistData)
        {
            if (!playlistData.IsOfType<PlaylistSaveData>(out var convertedPlaylist) || convertedPlaylist == null) return;

            Playlists.Remove(convertedPlaylist);
            SavePlaylists();
        }

        public void SavePlaylists()
        {
            foreach (var pl in Playlists)
            {
                Debug.WriteLine("Found playlist: " + pl.name + " Genres: " + pl.genres.Length + " Artists:" + pl.artists.Length + " Tracks:" + pl.tracks.Length);
            }

            DataStoreService.SavePlaylists([.. Playlists]);
        }

        void SelectPlaylist()
        {
            var playlist = SelectedItem;
            if (playlist == null || !playlist.IsOfType<PlaylistSaveData>(out var convertedPlaylist) || convertedPlaylist == null) return;

            var parameterViewModel = new ParameterViewModel(convertedPlaylist);
            parameterViewModel.PlaylistChanged += OnPlaylistChanged;
            Application.Current?.MainPage?.Navigation.PushAsync(new ParameterPage(parameterViewModel));
        }

        void OnPlaylistChanged()
        {
            SavePlaylists();
        }
    }
}
