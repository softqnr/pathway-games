using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using System.Drawing;

namespace PathwayGames.Services.Engangement
{
    public interface IEngagementService
    {
        void StartSession(float ppi, int sensitivity);

        double UpdateEngagement(FaceAnchorReading faceAnchorReading);

        double GetEngagement(FaceAnchorReading faceAnchorReading);

        Color GetEngagementColor(Tolerance tolerance);
    }
}
