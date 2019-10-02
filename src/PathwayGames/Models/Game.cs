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
        public SessionData SessionData { get; set; }

        public UserGameSettings GameSettings { get; set; }

        public Outcome Outcome { get; set; }

        public List<Slide> Slides { get; set; }

        public List<ButtonPress> ButtonPresses { get; set; }

        [JsonIgnore]
        public string GameDataFile { get; set; }

        [JsonIgnore]
        public string SensorDataFile { get; set; }

        public Game(GameType gameType, UserGameSettings gameSettings, long userId, string userName, string  seed)
        {
            Outcome = new Outcome();

            GameSettings = gameSettings;
            SessionData = new SessionData(gameType, userId, userName, seed);

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
