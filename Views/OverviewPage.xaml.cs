using SofyTrender.ViewModels;

namespace SofyTrender.Views;

public partial class OverviewPage : ContentPage
{
    public OverviewPage()
    {
        InitializeComponent();
        BindingContext = new OverviewViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        playlistsList.SelectedItem = null;
    }
}