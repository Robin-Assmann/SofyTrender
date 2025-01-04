using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SofyTrender.Models
{
    internal class GenreSearchModel : ISearchModel<string>
    {
        public ObservableCollection<string> Items { get; } = [];

        readonly List<string> _genres = [];

        public GenreSearchModel()
        {
            _ = ReadGenres();
        }

        public void Search(string input)
        {
            Items.Clear();
            foreach (var genre in _genres.Where(g => g.Contains(input, StringComparison.CurrentCultureIgnoreCase)))
            {
                Items.Add(genre);
            }
        }

        public void TextChange(string input)
        {
            Items.Clear();
            foreach (var genre in _genres.Where(g => g.Contains(input, StringComparison.CurrentCultureIgnoreCase)))
            {
                Items.Add(genre);
            }
        }

        async Task ReadGenres()
        {
            Task<List<string>> task = ReadLines("Genres.txt");
            await task;

            _genres.Clear();
            _genres.AddRange(task.Result);
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
                using StreamReader streamReader = new(fileStream);

                string? line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    rv.Add(line);
                }
                streamReader.Close();
                fileStream.Close();
                return rv;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return rv;
        }
    }
}
