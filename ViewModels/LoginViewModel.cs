using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SofyTrender.Models;
using SofyTrender.Services;
using SofyTrender.Views;

namespace SofyTrender.ViewModels
{
    partial class LoginViewModel : ObservableObject
    {
        public ICommand LoginCommand { get; set; }
        public ICommand AddCredentialsCommand { get; set; }

        [ObservableProperty] bool isInitialized;

        public LoginViewModel()
        {
            LoginCommand = new Command(Login, ()=> IsInitialized);
            AddCredentialsCommand = new Command(AddCredentials, ()=> !IsInitialized);

            SpotifyService.LoginStatusChanged += OnLoginStatusChanged;
            SpotifyService.InitChanged += OnInitChanged;

            OnInitChanged();
            LoadCredentials();
        }

        async void LoadCredentials()
        {
            var result = await DataStoreService.LoadCredentials();

            if (result == null)
            {
                return;
            }

            SpotifyService.Init(result);
        }

        void Login()
        {
            SpotifyService.Login();
        }

        async void AddCredentials()
        {
            var credentials = await ShowCredentialsPicker();

            if (credentials == null) {

                Application.Current?.MainPage.DisplayAlert("Playlist", $" Could not read the selected File! ", "Ok");

                return;
            }

            SpotifyService.Init(credentials);
            DataStoreService.SaveCredentials(credentials);
        }

        async Task<CredentialsData> ShowCredentialsPicker()
        {
            CredentialsData rv = default;
            try
            {
                var options = PickOptions.Default;
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("txt", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = await result.OpenReadAsync();

                        using (var sr = new StreamReader(stream))
                        {
                            var text = await sr.ReadToEndAsync();
                            rv = JsonConvert.DeserializeObject<CredentialsData>(text);
                            return rv;
                        }
                    }
                }

                return rv;
            } catch (Exception ex)
            {
                Application.Current?.MainPage.DisplayAlert("Playlist", $" Could not deserialize the selected File! ", "Ok");
            }

            return rv;
        }

        void OnLoginStatusChanged(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                Application.Current?.MainPage?.Navigation.PushAsync(new OverviewPage());
            }
        }

        void OnInitChanged()
        {
            IsInitialized = SpotifyService.IsInitialized;

            ((Command)LoginCommand).ChangeCanExecute();
            ((Command)AddCredentialsCommand).ChangeCanExecute();
        }
    }
}

