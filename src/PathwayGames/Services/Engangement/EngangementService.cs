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
        public double? CalculateEngangement(int sensitivity, FaceAnchorReading faceAnchorReading)
        {
            // Simulate engangement calculation for development purposes
            return (double?)faceAnchorReading.FacialExpressions["SmileLeft"];
        }

        public double GetEngangement()
        {
            // NOTE: For development purposes
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
