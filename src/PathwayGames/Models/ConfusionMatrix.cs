using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathwayGames.Models
{
    public class ConfusionMatrix
    {
        public int CorrectCommission { get; private set; }

        public int WrongCommission { get; private set; }

        public int CorrectOmission { get; private set; }

        public int WrongOmission { get; private set; }

        public int TotalPresses
        {
            get => CorrectCommission + WrongCommission;
        }

        public int TotalOmissions
        {
            get => CorrectOmission + WrongOmission;
        }

        public double H
        {
            get => (TotalPresses > 0) ? CorrectCommission / TotalPresses : 0;
        }
        public double FA {
            get => (TotalPresses > 0) ? WrongCommission / TotalPresses : 0;
        }

        public ConfusionMatrix(List<Slide> slides)
        {
            Calculate(slides);
        }

        private void Calculate(List<Slide> slides)
        {
            foreach (Slide slide in slides)
            {
                switch (slide.ResponseOutcome)
                {
                    case ResponseOutcome.CorrectCommission:
                        CorrectCommission++;
                        break;
                    case ResponseOutcome.WrongCommission:
                        WrongCommission++;
                        break;
                    case ResponseOutcome.CorrectOmission:
                        CorrectOmission++;
                        break;
                    case ResponseOutcome.WrongOmission:
                        WrongOmission++;
                        break;
                }
            }
        }
    }
}
