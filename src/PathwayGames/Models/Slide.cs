using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PathwayGames.Infrastructure.Json;
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

        [JsonIgnore]
        public double DisplayDuration { get; set; }

        public double BlankDuration { get; set; }

        [JsonIgnore]
        public string Image { get; set; }

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

        public Slide(SlideType slideType, double displayDuration)
        {
            SlideType = slideType;
            DisplayDuration = displayDuration;
        }
    }
}
