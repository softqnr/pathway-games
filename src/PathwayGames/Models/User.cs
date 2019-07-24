using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace PathwayGames.Models
{
    [Table("Users")]
    public class User : ModelBase
    {
        [Indexed]
        [Collation("NOCASE")]
        public string UserName { get; set; }

        [Indexed]
        [Collation("NOCASE")]
        public string UserType { get; set; }

        public bool IsSelected { get; set; }

        public string PIN { get; set; }

        [ForeignKey(typeof(UserSettings))]
        public long? UserSettingId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public UserSettings UserSettings { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<UserGameSession> GameSessions { get; set; }

        public User()
        {
            GameSessions = new List<UserGameSession>();
        }

        public UserGameSession AddGameSession(Game game, string gameDataFile, string sensorDataFile)
        {
            UserGameSession gameSession = new UserGameSession(this,
                game.GameType.ToString(),
                game.StartDate,
                game.EndDate.Value,
                gameDataFile,
                sensorDataFile);

            GameSessions.Add(gameSession);

            return gameSession;
        }

        public bool IsAdmin {
            get => UserType == Enums.UserType.Admin.ToString();
        }
    }
}
