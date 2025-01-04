using SofyTrender.ViewModels;

namespace SofyTrender.Pages;

public partial class SearchPage : ContentPage
{
    public event Action<object>? ItemSelected;

    public SearchPage()
    {
        InitializeComponent();
    }

    public void SetViewModel<T>(SearchViewModel<T> searchModel)
    {
        BindingContext = searchModel;
    }
}