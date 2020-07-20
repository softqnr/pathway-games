using PathwayGames.Models;
using PathwayGames.Sensors;

namespace PathwayGames.Services.Engangement
{
    public class EngangementService : IEngangementService
    {
        public LiveUserState liveUserState;

        public double? CalculateEngangement(FaceAnchorReading faceAnchorReading)
        {
            //// Simulate engangement calculation for development purposes
            //return (double?)faceAnchorReading.FacialExpressions["SmileLeft"];

            if (liveUserState == null)
                liveUserState = new LiveUserState();

            return liveUserState.GetState(faceAnchorReading);

        }
    }
}
