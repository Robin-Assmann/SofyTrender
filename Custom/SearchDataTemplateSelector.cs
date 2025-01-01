using SpotifyAPI.Web;

namespace SofyTrender.Pages
{
    public class SearchDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GenreTemplate { get; set; }
        public DataTemplate ArtistTemplate { get; set; }
        public DataTemplate TrackTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if(item.GetType() == typeof(FullArtist))
            {
                return ArtistTemplate;
            }

            if (item.GetType() == typeof(FullTrack))
            {
                return TrackTemplate;
            }

            return GenreTemplate;
        }
    }
}
