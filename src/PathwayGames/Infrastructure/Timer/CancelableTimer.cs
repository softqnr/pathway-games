using System;
using System.Threading;

namespace PathwayGames.Infrastructure.Timer
{
    public class CancelableTimer
    {
        private readonly TimeSpan _timeSpan;
        private readonly Action _callback;

        private static CancellationTokenSource _cancellationTokenSource;

        public CancelableTimer(TimeSpan timeSpan, Action callback)
        {
            _timeSpan = timeSpan;
            _callback = callback;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public void Start()
        {
            // Take a safe copy
            CancellationTokenSource cts = _cancellationTokenSource; 

            Xamarin.Forms.Device.StartTimer(_timeSpan, () =>
            {
                if (cts.IsCancellationRequested)
                {
                    // Stop
                    return false;
                }
                _callback.Invoke();
                
                // Run
                return true; 
            });
        }

        public void Stop()
        {
            Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();
        }
    }
}
