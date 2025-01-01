using System.Collections.ObjectModel;

namespace SofyTrender.Models
{
    public interface ISearchModel<T>
    {
        public ObservableCollection<T> Items { get; }
        public void Search(string input);
        public void TextChange(string input);
    }
}
