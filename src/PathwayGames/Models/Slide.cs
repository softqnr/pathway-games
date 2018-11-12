using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Models
{
    public class Slide
    {
        public SlideType SlideType { get; set; }
        public string Name { get; set; }
        public bool CorrectOutcome { get; set; }
        public double DisplayDuration { get; set; }
        public string Image { get; set; }
        public string Sound { get; set; }

        public Slide(SlideType slideType)
        {
            SlideType = slideType;
        }
    }
}
