using System;

namespace PathwayGames.Sensors
{
    public interface ISensor<T>
    {
        bool RecordingEnabled { get; set; }

        void OnReadingTaken(T e);

        void OnTrackingStarted(EventArgs e);

        void OnTrackingStopped(EventArgs e);
    }
}
