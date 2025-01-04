using SofyTrender.Models;
using SpotifyAPI.Web;

namespace SofyTrender.Services
{
    public static class SpotifyService
    {
        public static event Action<bool>? LoginStatusChanged;
        public static event Action? InitChanged;

        public static SpotifyClient? SpotifyClient { get; private set; }
        public static bool IsInitialized => _credentials != null;

        static CredentialsData? _credentials;

        public static void Init(CredentialsData data)
        {
            _credentials = data;

            //Needed to be called before Login to insure correct linking
            Scripts.WinUIEx.WebAuthenticator.CheckOAuthRedirectionActivation();

            InitChanged?.Invoke();
        }

        public static async void Login()
        {
            if (_credentials == null) return;

#if WINDOWS
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            var loginRequest = new LoginRequest(
              new Uri(_credentials.callbackAddress),
              _credentials.clientId,
              LoginRequest.ResponseType.Token
            )
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.PlaylistModifyPrivate, Scopes.PlaylistReadCollaborative }
            };
            var uri = loginRequest.ToUri();

            var authOptions = new WebAuthenticatorOptions();
            authOptions.Url = uri;
            authOptions.CallbackUrl = new Uri(_credentials.callbackAddress);
            authOptions.PrefersEphemeralWebBrowserSession = true;

            var response = await Scripts.WinUIEx.WebAuthenticator.AuthenticateAsync(uri, new Uri(_credentials.callbackAddress));

            SpotifyClient = new SpotifyClient(response.AccessToken);
            LoginStatusChanged?.Invoke(true);
#endif
        }

        public static void Logout()
        {
            SpotifyClient = null;
            LoginStatusChanged?.Invoke(false);
        }
    }
}
