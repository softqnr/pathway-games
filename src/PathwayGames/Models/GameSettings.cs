using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Models
{
    public class GameSettings
    {
        public double SlideDisplayDuration { get; set; } = 1.5;

        public double RewardDisplayDuration { get; set; } = 2;

        public int SlideCount { get; set; } = 10;

        public double BlankSlideDisplayTime { get; set; } = 2.0;

        public double BlankSlideDisplayTimeVariation { get; set; } = 0.1;

        public double[] BlankSlideDisplayTimes { get => new double[] { BlankSlideDisplayTime,
                BlankSlideDisplayTime + BlankSlideDisplayTimeVariation,
                BlankSlideDisplayTime - BlankSlideDisplayTimeVariation
            };
        }

        public int SeekGameRows { get; set; }

        public int SeekGameColumns { get; set; }
    }
}
