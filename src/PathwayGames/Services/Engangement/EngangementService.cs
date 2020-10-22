using PathwayGames.Extensions;
using PathwayGames.Helpers;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using System.Drawing;

namespace PathwayGames.Services.Engangement
{
    public class EngangementService : IEngangementService
    {
        LiveUserState _liveUserState;

        public EngangementService()
        {
            if (_liveUserState == null)
                _liveUserState = new LiveUserState();
        }

        public double? CalculateEngangement(int sensitivity, FaceAnchorReading faceAnchorReading)
        {
            //// Simulate engangement calculation for development purposes
            //return (double?)faceAnchorReading.FacialExpressions["SmileLeft"];

            return _liveUserState.GetState(faceAnchorReading);
        }

        //public double GetEngangement(int sensitivity, FaceAnchorReading faceAnchorReading)
        //{
        //    return _liveUserState.GetState(faceAnchorReading);
        //}

        public double GetEngangement()
        {
            // NOTE: For development purposes
            // here you should put engagement calc

            return ThreadSafeRandom.CurrentThreadRandom.NextDouble();
        }

        public Color GetEngangementColor(Tolerance tolerance)
        {
            double engangementValue = this.GetEngangement();

            Models.Engangement engangemet = new Models.Engangement(engangementValue, tolerance);

            // Blend the two range colors
            return engangemet.Color1.Blend(engangemet.Color2, engangemet.Delta);
        }
    }
}
