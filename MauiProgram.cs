using GaiaAgent.Pages;
using GaiaAgent.Services;
using Microsoft.Extensions.Logging;

namespace GaiaAgent;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Dependency Injection
        builder.Services.AddSingleton<MapAgent>();
        builder.Services.AddHttpClient<MapAgent>();   // ← Requires Microsoft.Extensions.Http

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
