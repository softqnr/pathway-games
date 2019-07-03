using System;

namespace PathwayGames.Controls
{
    public class LookAtPointEventArgs : EventArgs
    {
        public double PointX { get; set; }
        public double PointY { get; set; }
        public double Distance { get; set; }

        public LookAtPointEventArgs(double pointX, double pointY, double distance)
        {
            PointX = pointX;
            PointY = pointY;
            Distance = distance;
        }
    }
}
