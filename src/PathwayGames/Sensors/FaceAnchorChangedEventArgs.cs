using System;

namespace PathwayGames.Sensors
{
    public class FaceAnchorChangedEventArgs : EventArgs
    {
        public FaceAnchorChangedEventArgs(FaceAnchorReading reading) => Reading = reading;

        public FaceAnchorReading Reading { get; }
    }
}
