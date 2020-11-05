using PathwayGames.Extensions;
using PathwayGames.Helpers;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using System;
using System.Diagnostics;
using System.Drawing;

namespace PathwayGames.Services.Engangement
{
    public class EngangementService : IEngangementService
    {
        LiveUserState _liveUserState;

        public void InitEngagement(LiveUserState liveUserState)
        {
            _liveUserState = liveUserState;
        }

        public double? CalculateEngangement(int sensitivity, FaceAnchorReading faceAnchorReading)
        {
            //// Simulate engangement calculation for development purposes
            //return (double?)faceAnchorReading.FacialExpressions["SmileLeft"];

            return _liveUserState.UpdateState(faceAnchorReading);
        }

        //public double GetEngangement(int sensitivity, FaceAnchorReading faceAnchorReading)
        //{
        //    return _liveUserState.GetState(faceAnchorReading);
        //}

        public double GetEngangement()
        {
            // NOTE: For development purposes
            // here you should put engagement calc

            Debug.Write(_liveUserState.EngagementScore);

            return _liveUserState.EngagementScore;
        }

        public Color GetEngangementColor(Tolerance tolerance)
        {
            double engangementValue = this.GetEngangement();

            Models.Engangement engangemet = new Models.Engangement(engangementValue, tolerance);

            // Blend the two range colors
            return engangemet.Color2.Blend(engangemet.Color1, engangemet.Delta);
        }
    }
}
