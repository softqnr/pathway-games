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

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<UserGameSession> GameSessions { get; set; }
    }
}
