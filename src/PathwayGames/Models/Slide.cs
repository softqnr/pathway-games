﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Models
{
    public class Slide
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SlideType SlideType { get; set; }

        public string Name { get; set; }

        public double DisplayDuration { get; set; }

        public double BlankDuration { get; set; }

        public string Image { get; set; }

        public string Sound { get; set; }

        public DateTime SlideDisplayed { get; set; }

        public DateTime? SlideHidden { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseOutcome ResponseOutcome { get; set; }

        public TimeSpan ResponseTime { get; set; }

        public int Points { get; set; }

        public Slide(SlideType slideType, double displayDuration)
        {
            SlideType = slideType;
            DisplayDuration = displayDuration;
        }
    }
}
