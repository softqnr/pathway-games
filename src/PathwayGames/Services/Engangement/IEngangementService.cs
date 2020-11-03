using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using System.Drawing;

namespace PathwayGames.Services.Engangement
{
    public interface IEngangementService
    {
        void InitEngagement(LiveUserState liveUserState);

        double? CalculateEngangement(int sensitivity, FaceAnchorReading faceAnchorReading);

        double GetEngangement();

        Color GetEngangementColor(Tolerance tolerance);
    }
}
