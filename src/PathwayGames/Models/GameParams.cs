using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Models
{
    public class GameParams
    {
        public double SlideTimeout { get; set; }

        public double RewardSlideDuration { get; set; }

        public bool ShowBlankSlide { get; set; }

        public double[] BlankSlideDurations { get; set; }
    }
}
