using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using SofyTrender.Models;

namespace SofyTrender.ViewModels
{
    public class SearchViewModel<T> : ObservableObject
    {
        public ICommand SearchCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand TextChangedCommand { get; set; }

        public ISearchModel<T> SearchModel { get; private set; }
        public ObservableCollection<T> Items => SearchModel.Items;

        public event Action<object> ItemSelected;

        public SearchViewModel(ISearchModel<T> searchModel)
        {
            SearchModel = searchModel;
            SearchCommand = new Command(Search);
            SelectCommand = new Command(Select);
            TextChangedCommand = new Command(TextChange);
        }

        void Search(object input)
        {
            SearchModel.Search((string)input);
        }

        void TextChange(object input)
        {
            SearchModel.Search((string)input);
        }

        void Select(object selectedItem)
        {
            Debug.WriteLine("Selected Item: " + selectedItem);
            ItemSelected?.Invoke(selectedItem);
        }
    }
}
