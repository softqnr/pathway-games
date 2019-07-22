using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace PathwayGames.Models
{
    [Table("UserGameSessions")]
    public class UserGameSession : ModelBase
    {
        [Indexed]
        [ForeignKey(typeof(User))]
        public long UserId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public User User { get; set; }

        public string GameType { get; set; }

        public DateTime DateStarted { get; set; }

        public DateTime DateEnded { get; set; }

        public string GameDataFile { get; set; }

        public string SensorDataFile { get; set; }
    }
}
