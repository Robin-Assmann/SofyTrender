using System.Globalization;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace SofyTrender
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMauiHandlers(_ =>
                {
                    ButtonHandler.Mapper.AppendToMapping(
                        nameof(View.BackgroundColor),
                        (handler, View) => handler.UpdateValue(nameof(IView.Background)));
                })
                .UseMauiCommunityToolkit(options =>
                {
                    options.SetShouldEnableSnackbarOnWindows(true);
                })
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
            SwitchHandler.Mapper.AppendToMapping("NoLabel", (handler, view) => {
                // Remove this if statement if you want to apply this to all switches
                if (view is Switch)
                {
                    handler.PlatformView.OnContent = null;
                    handler.PlatformView.OffContent = null;

                    // Add this to remove the padding around the switch as well
                    // handler.PlatformView.MinWidth = 0;
                }
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
