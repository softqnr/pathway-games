using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using System.Drawing;

namespace PathwayGames.Services.Engangement
{
    public interface IEngangementService
    {
        double? CalculateEngangement(int sensitivity, FaceAnchorReading faceAnchorReading);

        double GetEngangement();

        Color GetEngangementColor(Tolerance tolerance);
    }
}
