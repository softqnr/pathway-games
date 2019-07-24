using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PathwayGames.Models
{
    [Table("UserSettings")]
    public class UserSettings : ModelBase
    {
        [Indexed]
        [ForeignKey(typeof(User))]
        public long UserId { get; set; }

        [OneToOne]
        public User User { get; set; }

        public bool EyeGazeSensor { get; set; }

        public bool EEGSensor { get; set; }

        public bool AccelerationSensor { get; set; }


        public double SlideDisplayDuration { get; set; }

        public double RewardDisplayDuration { get; set; }

        public double BlankSlideDisplayTime { get; set; }

        public double BlankSlideDisplayTimeVariation { get; set; }

        public UserSettings()
        {

        }
    }
}
