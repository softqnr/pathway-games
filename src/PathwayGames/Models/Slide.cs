﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PathwayGames.Infrastructure.Json;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;

namespace PathwayGames.Models
{
    public class Slide
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SlideType SlideType { get; set; }

        [JsonIgnore]
        public double DisplayDuration { get; set; }

        public double BlankDuration { get; set; }

        [JsonIgnore]
        public IList<string> Images { get; set; }

        [JsonIgnore]
        public string BorderColor { get; set; }

        [JsonIgnore]
        public string Sound { get; set; }

        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime SlideDisplayed { get; set; }

        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? SlideHidden { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseOutcome ResponseOutcome { get; set; }

        public TimeSpan ResponseTime { get; set; }

        public int Points { get; set; }

        public Slide(SlideType slideType, double displayDuration, string slideImage) : this(slideType, displayDuration, slideImage, 0, "")
        { }

        public Slide(SlideType slideType, double displayDuration, string slideImage, double blankDuration) : this(slideType, displayDuration, slideImage, blankDuration, "")
        { }

        public Slide(SlideType slideType, double displayDuration, string slideImage, double blankDuration, string sound) : this(slideType, displayDuration, new List<string>{ slideImage }, blankDuration, sound)
        { }

        public Slide(SlideType slideType, double displayDuration, List<string> slideImages, double blankDuration) : this(slideType, displayDuration, slideImages, blankDuration, "")
        { }

        public Slide(SlideType slideType, double displayDuration, List<string> slideImages, double blankDuration, string sound)
        {
            Images = slideImages;
            SlideType = slideType;
            DisplayDuration = displayDuration;
            BlankDuration = blankDuration;
            Sound = sound;
        }
    }
}
