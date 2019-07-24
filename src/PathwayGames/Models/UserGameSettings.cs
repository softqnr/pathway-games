using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PathwayGames.Models
{
    [Table("UserGameSettings")]
    public class UserGameSettings : ModelBase
    {
        [Indexed]
        [ForeignKey(typeof(User))]
        public long UserId { get; set; }

        public bool EyeGazeSensor { get; set; }

        public bool EEGSensor { get; set; }

        public bool AccelerationSensor { get; set; }

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

        public UserGameSettings()
        {

        }
    }
}
