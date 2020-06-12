using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PathwayGames.Infrastructure.Json;
using PathwayGames.Models.Enums;
using System;

namespace PathwayGames.Models
{
    public class SessionData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public GameType GameType { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string Seed { get; set; }

        public DateTime StartDate { get; set; }

        [JsonConverter(typeof(UnixTimeMillisecondsConverter))]
        public DateTime StartTime { get => StartDate; }

        public DateTime? EndDate { get; set; }

        public string ApplicationVersion { get; set; }

        public SessionData(GameType gameType, long userId, string userName, string seed)
        {
            GameType = gameType;
            UserId = userId;
            UserName = userName;
            Seed = seed;
            ApplicationVersion = App.ApplicationVersion;
        }
    }
}
