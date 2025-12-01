namespace GaiaAgent.Models;

public class RouteResult
{
    public double DistanceMeters { get; set; }
    public double DurationSeconds { get; set; }
    public List<RoutePoint> Points { get; set; } = new();
}

public class RoutePoint
{
    public double Lat { get; set; }
    public double Lon { get; set; }

    public RoutePoint(double lat, double lon)
    {
        Lat = lat;
        Lon = lon;
    }
}