namespace GaiaAgent.Models;

public record OrientationResult(
    double Bearing,
    double RelativeAngle,
    string Side
);