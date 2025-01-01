using SofyTrender.ViewModels;

namespace SofyTrender.Pages
{
    public partial class ParameterPage : ContentPage
    {
        public ParameterPage(ParameterViewModel viewmodel)
        {
            InitializeComponent();

            BindingContext = viewmodel;
        }
    }
}