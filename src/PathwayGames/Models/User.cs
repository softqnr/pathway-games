using PathwayGames.Models.Enums;
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

        public string PIN { get; set; } = "";

        public Languages Language { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public UserGameSettings UserSettings { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<UserGameSession> GameSessions { get; set; }

        public User()
        {
            GameSessions = new List<UserGameSession>();
            UserSettings = new UserGameSettings();
        }

        public UserGameSession AddGameSession(Game game, string gameDataFile)
        {
            UserGameSession gameSession = new UserGameSession(this,
                game.SessionData.GameType.ToString(),
                game.SessionData.StartDate,
                game.SessionData.EndDate.Value,
                gameDataFile);

            GameSessions.Add(gameSession);

            return gameSession;
        }

        public bool IsAdmin {
            get => UserType == Enums.UserType.Admin.ToString();
        }
    }
}
