using System;
using System.Drawing;

namespace PathwayGames.Models
{
    public class ColorInterpolationParams
    {
        public Color EndPoint1 { get; }

        public Color EndPoint2 { get;  }

        public double Lamda { get;  }

        public ColorInterpolationParams(Color endPoint1, Color endPoint2, double lamda)
        {
            if (lamda < 0 || lamda > 1)
            {
                throw new ArgumentOutOfRangeException("lamda");
            }
            Lamda = lamda;
            EndPoint1 = endPoint1;
            EndPoint2 = endPoint2;
        }
    }
}
