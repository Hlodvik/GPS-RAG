namespace GaiaAgent.Models;

public record PoiResult(
    string Name,
    double Lat,
    double Lon,
    string Address,
    double DistanceMeters
);
