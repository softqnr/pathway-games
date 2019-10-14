using PathwayGames.Models;
using System.Collections.Generic;

namespace PathwayGames.Services.Engangement
{
    public interface IEngangementService
    {
        (double z, double y) CalculateZValue(double prob);
    }
}
