using PathwayGames.Helpers;
using System.Collections.Generic;

namespace PathwayGames.Extensions
{
    public static class IListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            // Fisher-Yates shuffle
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.CurrentThreadRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
