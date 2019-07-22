using System;

namespace PathwayGames.Controls
{
    public class EyeGazeChangedEventArgs : EventArgs
    {
        public double PointX { get; set; }
        public double PointY { get; set; }
        public double Distance { get; set; }

        public EyeGazeChangedEventArgs(double pointX, double pointY, double distance)
        {
            PointX = pointX;
            PointY = pointY;
            Distance = distance;
        }
    }
}
