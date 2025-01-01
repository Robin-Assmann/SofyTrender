using System.Collections.ObjectModel;
using SofyTrender.ViewModels;

namespace SofyTrender.Pages;

public partial class SearchPage : ContentPage
{
    public event Action<object> ItemSelected;

    public SearchPage()
    {
        InitializeComponent();
    }

    public void SetViewModel<T>(SearchViewModel<T> searchModel)
    {
        BindingContext = searchModel;
    }

    public void SetData<T>(ObservableCollection<T> items)
    {
        //genreList.ItemsSource = items;
    }

    private void OnListItemSelected(object sender, SelectionChangedEventArgs args)
    {
        //ItemSelected?.Invoke(args.CurrentSelection.First());
    }

    void OnSearchClicked(object sender, EventArgs eventArgs)
    {
        //_searchModel.Search(((SearchBar)sender).Text);
    }

    void OnTextChanged(object sender, TextChangedEventArgs eventArgs)
    {
        //_searchModel.TextChange(eventArgs.NewTextValue);
    }
}