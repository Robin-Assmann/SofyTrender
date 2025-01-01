using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SofyTrender.Models
{
    internal class GenreSearchModel : ISearchModel<string>
    {
        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

        List<string> _genres;

        public GenreSearchModel()
        {
            _ = ReadGenres();
        }

        public void Search(string input)
        {
            Debug.WriteLine("GenreSearchModel:Search " + input);
            Items.Clear();
            foreach (var genre in _genres.Where(g => g.Contains(input, StringComparison.CurrentCultureIgnoreCase)))
            {
                Items.Add(genre);
            }
        }

        public void TextChange(string input)
        {
            Debug.WriteLine("GenreSearchModel:TextChange " + input);
            Items.Clear();
            Debug.WriteLine("GenreSearchModel:_genres " + _genres.Count);
            foreach (var genre in _genres.Where(g => g.Contains(input, StringComparison.CurrentCultureIgnoreCase)))
            {
                Items.Add(genre);
            }
        }

        async Task ReadGenres()
        {
            Task<List<string>> task = ReadLines("Genres.txt");
            await task;
            _genres = task.Result;
            Debug.WriteLine("After Read: " + _genres.Count);
            foreach (var genre in _genres)
            {
                Items.Add(genre);
            }
        }

        async Task<List<string>> ReadLines(string DataFile)
        {
            var rv = new List<string>();
            try
            {
                // Open the source file
                using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(DataFile);
                using StreamReader sr = new(fileStream);

                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    rv.Add(line);
                }
                sr.Close();
                fileStream.Close();
                return rv;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return rv;
        }
    }
}
