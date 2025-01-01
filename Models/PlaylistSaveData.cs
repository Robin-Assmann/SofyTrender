using SofyTrender.Pages;
using SpotifyAPI.Web;

namespace SofyTrender.Models
{
    public class PlaylistSaveData
    {
        public string name { get; set; } = "New Playlist";
        public string playlistId = string.Empty;
        public FullPlaylist[] generatedPlaylists = Array.Empty<FullPlaylist>();
        public FullArtist[] artists = Array.Empty<FullArtist>();
        public FullTrack[] tracks = Array.Empty<FullTrack>();
        public string[] genres = Array.Empty<string>();
        public bool usePopularity = false;
        public int popularity = 50;
        public bool useYears = false;
        public int minYear = Utils.MinYear;
        public int maxYear = Utils.MaxYear;
    }
}
