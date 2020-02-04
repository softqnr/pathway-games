using PathwayGames.Sensors;

namespace PathwayGames.Services.Engangement
{
    public interface IEngangementService
    {
        double? CalculateEngangement(FaceAnchorReading faceAnchorReading);
    }
}
