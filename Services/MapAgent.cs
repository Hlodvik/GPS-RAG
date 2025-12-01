using System.Net.Http.Json;
using Microsoft.Maui.Devices.Sensors;
using GaiaAgent.Models;

namespace GaiaAgent.Services;

public class MapAgent
{
    private readonly HttpClient _http;

    public MapAgent(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://localhost:7049/");
    }

    // ------------------------------
    // GET DEVICE LOCATION
    // ------------------------------
    public async Task<Location?> GetLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(
                GeolocationAccuracy.Best,
                TimeSpan.FromSeconds(3)
            );

            return await Geolocation.Default.GetLocationAsync(request);
        }
        catch
        {
            return null;
        }
    }

    // ------------------------------
    // GET DEVICE HEADING (COMPASS)
    // ------------------------------
    public double? LastHeading { get; private set; }

    public async Task StartCompassAsync()
    {
        if (!Compass.Default.IsMonitoring)
        {
            Compass.Default.ReadingChanged += (_, e) =>
            {
                LastHeading = e.Reading.HeadingMagneticNorth;
            };

            Compass.Default.Start(SensorSpeed.Game);
        }
    }

    // ------------------------------
    // SEARCH
    // ------------------------------
    public async Task<List<PoiResult>?> SearchAsync(string query, double lat, double lon)
    {
        var url = $"search?query={query}&lat={lat}&lon={lon}";
        return await _http.GetFromJsonAsync<List<PoiResult>>(url);
    }

    // ------------------------------
    // ROUTE
    // ------------------------------
    public async Task<RouteResult?> GetRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
    {
        var url = $"route?fromLat={fromLat}&fromLon={fromLon}&toLat={toLat}&toLon={toLon}";
        return await _http.GetFromJsonAsync<RouteResult>(url);
    }

    // ------------------------------
    // ORIENTATION
    // ------------------------------
    public async Task<OrientationResult?> GetOrientationAsync(
        double userLat, double userLon, double heading, double targetLat, double targetLon)
    {
        var url =
            $"orientation?userLat={userLat}&userLon={userLon}&heading={heading}&targetLat={targetLat}&targetLon={targetLon}";

        return await _http.GetFromJsonAsync<OrientationResult>(url);
    }
}
