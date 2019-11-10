using System;
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
    }
}
