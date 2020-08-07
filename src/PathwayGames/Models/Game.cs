using Newtonsoft.Json;
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

        public SensoryData SensoryData { get; set; }

        [JsonIgnore]
        public string GameDataFile { get; set; }

        public Game(GameType gameType, UserGameSettings gameSettings, long userId, string userName, string  seed)
        {
            Outcome = new Outcome();

            GameSettings = gameSettings;

            SessionData = new SessionData(gameType, userId, userName, seed);

            SensoryData = new SensoryData();
        }

        public void RecordButtonPress(int slideIndex, Point p, DateTime dateTimePressed)
        {
            SensoryData.ButtonPresses.Add(new ButtonPress()
            {
                Coordinates = p,
                Time = dateTimePressed,
                SlideIndex = slideIndex
            });
        }
    }
}
