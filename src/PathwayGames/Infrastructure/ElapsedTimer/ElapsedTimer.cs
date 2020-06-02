using System;
using System.Diagnostics;
using System.Threading;

namespace PathwayGames.Infrastructure.Timer
{
    public class ElapsedTimer : IElapsedTimer
    {
        static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        long _lastResetTime;

        static double Frequency = (double)Stopwatch.Frequency;

        public ElapsedTimer()
        {
            Reset();
        }

        public void Reset()
        {
            Interlocked.Exchange(ref _lastResetTime, Stopwatch.GetTimestamp());
        }

        public double SecondsElapsed
        {
            get
            {
                var resetTime = Interlocked.Read(ref _lastResetTime);
                // Divide ticks by Stopwatch.Frequency to get seconds
                return (Stopwatch.GetTimestamp() - resetTime) / Frequency;
            }
        }
    }
}
