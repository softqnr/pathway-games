using Newtonsoft.Json;
using PathwayGames.Models.Enums;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PathwayGames.Models
{
    [Table("UserGameSettings")]
    public class UserGameSettings : ModelBase
    {
        [JsonIgnore]
        [Indexed]
        [ForeignKey(typeof(User))]
        public long UserId { get; set; }

        [Indexed]
        [ForeignKey(typeof(SeekGridOption))]
        public long SeekGridOptionId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public SeekGridOption SeekGridOptions { get; set; }

        public bool EyeGazeSensor { get; set; } = true;

        public bool EEGSensor { get; set; } = true;

        public bool AccelerationSensor { get; set; } = true;

        public double SlideDisplayDuration { get; set; } = 1.5;

        public double BlankSlideDisplayTime { get; set; } = 1.0;

        public double BlankSlideDisplayTimeVariation { get; set; } = 0.1;

        public double[] BlankSlideDisplayTimes
        {
            get => new double[] { BlankSlideDisplayTime,
                BlankSlideDisplayTime + BlankSlideDisplayTimeVariation,
                BlankSlideDisplayTime - BlankSlideDisplayTimeVariation
            };
        }

        public double RewardDisplayDuration { get; set; } = 2;

        public int SlideCount { get; set; } = 10;

        public bool EyeGazeVisualisation { get; set; }

        public int ScreenPPI { get; set; }

        public float VisualizationWidthCompensation { get; set; }

        public float VisualizationHeightCompensation { get; set; }

        public int LiveViewSensitivity { get; set; } = 5;

        public string LiveViewTolerance { get; set; } = Tolerance.Medium.ToString();

        public UserGameSettings()
        {

        }
    }
}
