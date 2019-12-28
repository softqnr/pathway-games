using System;
using System.Linq;
using System.Threading;

namespace PathwayGames.Helpers
{
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random LocalRandom;

        public static Random CurrentThreadRandom
        {
            get {
                return LocalRandom ?? (LocalRandom = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));
            }
        }

        public static double GetRandomNumber(double[] values)
        {
            int index = CurrentThreadRandom.Next(0, values.Count());
            return values[index];
        }

        public static string CreateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[CurrentThreadRandom.Next(s.Length)]).ToArray());
        }
    }
}
