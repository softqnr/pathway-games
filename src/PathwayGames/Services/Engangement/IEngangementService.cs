using PathwayGames.Models;
using System.Collections.Generic;

namespace PathwayGames.Services.Engangement
{
    public interface IEngangementService
    {
        ConfusionMatrix CalculateConfusionMatrix(List<Slide> slides);
        (double z, double y) CalculateZValue(double prob);

    }
}
