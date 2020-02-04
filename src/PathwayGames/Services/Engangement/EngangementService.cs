﻿using PathwayGames.Sensors;

namespace PathwayGames.Services.Engangement
{
    public class EngangementService : IEngangementService
    {
        public double? CalculateEngangement(FaceAnchorReading faceAnchorReading)
        {
            // Simulate engangement calculation for development purposes
            return (double?)faceAnchorReading.FacialExpressions["SmileLeft"];
        }
    }
}
