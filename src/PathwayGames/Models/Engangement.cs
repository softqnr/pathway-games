using PathwayGames.Models.Enums;
using System.Drawing;

namespace PathwayGames.Models
{
    public class Engangement
    {
        public double Value { get; private set; }

        public double Delta { 
            get
            {
                return (Value - RangeStart) / (RangeEnd - RangeStart);
            }
        }

        public Tolerance Tolerance { get; private set; }

        public Color Color1 { get; private set; }

        public Color Color2 { get; private set; }

        public double RangeStart { get; private set; }

        public double RangeEnd { get; private set; }

        public Engangement(double engangement, Tolerance tolerance)
        {
            Value = engangement;
            Tolerance = tolerance;

            Init();
        }

        private void Init()
        {
            double range1Max = 0;
            double range2Max = 0;

            switch (Tolerance)
            {
                case Tolerance.Low:
                    // 3x Red => Range(0, 0.50)
                    // 2x Amber => Range(0.50, 0.83)
                    // 1x Green => Range(0.83, 1)
                    range1Max = 0.5;
                    range2Max = 0.83;
                    break;
                case Tolerance.Medium:
                    // 2x Red => Range(0, 0.44)
                    // 1.5x Amber => Range(0.44, 0.77)
                    // 1x Green => Range(0.77, 1)
                    range1Max = 0.44;
                    range2Max = 0.77;
                    break;
                case Tolerance.High:
                    // 1x Red => Range(0, 0.33)
                    // 1x Amber => Range(0.33, 0.66)
                    // 1x Green => Range(0.66, 1)
                    range1Max = 0.33;
                    range2Max = 0.66;
                    break;
            }

            if (0 <= Value && Value < range1Max)
            {
                RangeStart = 0;
                RangeEnd = range1Max;

                Color1 = Color.Red;
                Color2 = Color.Orange;
            }
            else if (range1Max <= Value && Value <= range2Max)
            {
                RangeStart = range1Max;
                RangeEnd = range2Max;

                Color1 = Color.Orange;
                Color2 = Color.Yellow;
            }
            else
            {
                RangeStart = range2Max;
                RangeEnd = 1;

                Color1 = Color.Yellow;
                Color2 = Color.Green;
            }
        }
    }
}
