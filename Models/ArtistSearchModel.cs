using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SofyTrender.Services;
using SpotifyAPI.Web;

namespace SofyTrender.Models
{
    internal class ArtistSearchModel : ISearchModel<FullArtist>
    {
        public ObservableCollection<FullArtist> Items { get; } = new ObservableCollection<FullArtist>();

        public ArtistSearchModel()
        {
        }

        public void Search(string input)
        {
            Debug.WriteLine("ArtistSearchModel:Search " + input);
            DoSearch();

            async void DoSearch()
            {
                Items.Clear();
                var searchRequest = new SearchRequest(SearchRequest.Types.Artist, input);
                var search = await SpotifyService.SpotifyClient.Search.Item(searchRequest);
                if (search.Artists.Items != null)
                {
                    foreach (var artist in search.Artists.Items)
                    {
                        Items.Add(artist);
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
