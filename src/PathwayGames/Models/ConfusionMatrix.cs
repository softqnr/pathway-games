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
        public int TotalPresses { get; private set; }
        public int TotalOmissions { get; private set; }

        public double H
        {
            get
            {
                return CorrectCommission / TotalPresses;
            }
        }
        public double FA {
            get
            {
                return WrongCommission / TotalPresses;
            }
        }

        public ConfusionMatrix(List<Slide> slides)
        {
            Parse(slides);
        }

        private void Parse(List<Slide> slides)
        {
            foreach (Slide slide in slides)
            {
                switch (slide.ResponseOutcome)
                {
                    case ResponseOutcome.CorrectCommission:
                        CorrectCommission++;
                        TotalPresses++;
                        break;
                    case ResponseOutcome.WrongCommission:
                        WrongCommission++;
                        TotalOmissions++;
                        break;
                    case ResponseOutcome.CorrectOmission:
                        CorrectOmission++;
                        TotalOmissions++;
                        break;
                    case ResponseOutcome.WrongOmission:
                        WrongOmission++;
                        TotalPresses++;
                        break;
                }
              
            }
        }
    }
}
