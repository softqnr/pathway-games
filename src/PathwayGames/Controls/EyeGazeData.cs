using System;

namespace PathwayGames.Controls
{
    public class EyeGazeData
    {
        public DateTime ReadingDateTime { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Distance { get; set; }

        public EyeGazeData(DateTime readingDateTime, double pointX, double pointY, double distance)
        {
            ReadingDateTime = readingDateTime;
            X = pointX;
            Y = pointY;
            Distance = distance;
        }

        public override string ToString()
        {
            return $"{ReadingDateTime:dd/MM/yyyy HH:mm:ss.ffff}, {X}, {Y}, {Distance}";
        }
    }
}
