using PathwayGames.Models;
using System;

namespace PathwayGames.Controls
{
    public class EyeGazeChangedEventArgs : EventArgs
    {
        public EyeGazeChangedEventArgs(FaceAnchorData reading) => Reading = reading;

        public FaceAnchorData Reading { get; }
    }
}
