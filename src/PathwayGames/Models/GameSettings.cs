using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Models
{
    public class GameSettings
    {
        public double SlideDisplayDuration { get; set; }

        public int SlideCount { get; set; }

        public double[] BlankSlideDisplayTimes { get; set; }

        public int SeekGameRows { get; set; }

        public int SeekGameColumns { get; set; }
    }
}
