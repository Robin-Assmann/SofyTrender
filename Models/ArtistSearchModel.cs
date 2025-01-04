using System.Collections.ObjectModel;
using System.Diagnostics;
using SofyTrender.Services;
using SpotifyAPI.Web;

namespace SofyTrender.Models
{
    internal class ArtistSearchModel : ISearchModel<FullArtist>
    {
        public ObservableCollection<FullArtist> Items { get; } = [];

        public void Search(string input)
        {
            Debug.WriteLine("ArtistSearchModel:Search " + input);
            DoSearch();

            async void DoSearch()
            {
                Items.Clear();
                var searchRequest = new SearchRequest(SearchRequest.Types.Artist, input);
                var searchResponse = await SpotifyService.SpotifyClient?.Search.Item(searchRequest) ?? null;
                if (searchResponse != null && searchResponse.Artists.Items != null)
                {
                    foreach (var artist in searchResponse.Artists.Items)
                    {
                        Items.Add(artist);
                    }
                }
            }
        }

        public void TextChange(string input)
        {
            //noop
        }
    }
}
