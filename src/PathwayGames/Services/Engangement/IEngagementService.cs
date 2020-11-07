using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using System.Drawing;

namespace PathwayGames.Services.Engangement
{
    public interface IEngagementService
    {
        void Init(float ppi);

        double UpdateEngagement(FaceAnchorReading faceAnchorReading);

        double GetEngagement(/*int sensitivity,*/ FaceAnchorReading faceAnchorReading);

        Color GetEngagementColor(Tolerance tolerance);
    }
}
