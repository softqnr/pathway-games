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

        public GameSettings GameSettings { get; set; }

        public string UserName { get; set; }

        public string Seed { get; set; }

        public List<Slide> Slides { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<ButtonPress> ButtonPresses { get; set; }

        public ConfusionMatrix ConfusionMatrix { get; set; }

        public int Score { get; set; }

        public double ScorePercentage { get; set; }

        public TimeSpan AverageResponseTime { get; set; }

        public TimeSpan AverageResponseTimeCorrect { get; set; }

        public TimeSpan AverageResponseTimeWrong { get; set; }

        public string GameDataFile { get; set; }

        public string SensorDataFile { get; set; }

        public Game(GameType gameType,GameSettings gameSettings, string userName, string  seed)
        {
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
