using System;
using System.Diagnostics;
using System.Threading;

namespace PathwayGames.Infrastructure.Timer
{
    public sealed class TimerClock : IDisposable
    {
        private const long MaxIdleTime = TimeSpan.TicksPerSecond * 10;
        private const long TicksMultiplier = 1000 * TimeSpan.TicksPerMillisecond;

        private static readonly ThreadLocal<DateTime> _startTime =
            new ThreadLocal<DateTime>(() => DateTime.UtcNow, false);

        private static readonly ThreadLocal<double> _startTimestamp =
            new ThreadLocal<double>(() => Stopwatch.GetTimestamp(), false);

        public static DateTime UtcNow
        {
            get
            {
                double endTimestamp = Stopwatch.GetTimestamp();

                var durationInTicks = (endTimestamp - _startTimestamp.Value) / Stopwatch.Frequency * TicksMultiplier;
                if (durationInTicks >= MaxIdleTime)
                {
                    // Stopwatch after some time looses accuracy so reset it
                    _startTimestamp.Value = Stopwatch.GetTimestamp();
                    _startTime.Value = DateTime.UtcNow;
                    return _startTime.Value;
                }

                return _startTime.Value.AddTicks((long)durationInTicks);
            }
        }

        public static DateTime Now
        {
            get
            {
                return UtcNow.ToLocalTime();
            }
        }

        public void Dispose()
        {
            _startTime.Dispose();
            _startTimestamp.Dispose();
        }
    }
}
