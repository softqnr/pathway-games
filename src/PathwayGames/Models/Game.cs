using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PathwayGames.Models
{
    public class Game
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public GameType GameType { get; set; }

        public UserGameSettings GameSettings { get; set; }

        public Outcome Outcome { get; set; }

        public string UserName { get; set; }

        public string Seed { get; set; }

        public List<Slide> Slides { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<ButtonPress> ButtonPresses { get; set; }

        [JsonIgnore]
        public string GameDataFile { get; set; }

        [JsonIgnore]
        public string SensorDataFile { get; set; }

        public Game(GameType gameType, UserGameSettings gameSettings, string userName, string  seed)
        {
            Outcome = new Outcome();

            GameType = gameType;
            GameSettings = gameSettings;
            UserName = userName;
            Seed = seed;

            ButtonPresses = new List<ButtonPress>();
        }

        public void RecordButtonPress(Int32? slideIndex, Point p)
        {
            ButtonPresses.Add(new ButtonPress()
            {
                Coordinates = p,
                Time = DateTime.Now,
                SlideIndex = slideIndex
            });
        }
    }
}
