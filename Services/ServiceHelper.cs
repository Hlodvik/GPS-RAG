namespace GaiaAgent;

public static class ServiceHelper
{
    public static IServiceProvider Services =>
        IPlatformApplication.Current.Services;

    private static IPlatformApplication Current =>
        (IPlatformApplication)Application.Current!;
}