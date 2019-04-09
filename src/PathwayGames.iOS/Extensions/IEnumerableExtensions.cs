using System;
using System.Collections.Generic;

namespace PathwayGames.iOS.Extensions
{
    public static class IEnumerableExtensions
    {
        public static float Average(this IEnumerable<nfloat> positions)
        {
            float result = 0;
            float sum = 0;
            float count = 0;
            foreach (float e in positions)
            {
                sum += e;
                count++;
            }
            result = (count > 0) ? sum / count : 0;
            return result;
        }
    }
}