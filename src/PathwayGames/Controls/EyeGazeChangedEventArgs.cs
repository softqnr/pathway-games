using System;

namespace PathwayGames.Controls
{
    public class EyeGazeChangedEventArgs : EventArgs
    {
        public EyeGazeChangedEventArgs(EyeGazeData reading) => Reading = reading;

        public EyeGazeData Reading { get; }
    }
}
