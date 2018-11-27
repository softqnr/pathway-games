using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Models
{
    public class Game
    {
        public GameType GameType { get; set; }
        public string Seed { get; set; }
        public IList<Slide> Slides { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ButtonPress> ButtonPresses { get; set; }
        public double Score { get; set; }
        public double ScorePercentage { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public TimeSpan AverageResponseTimeCorrect { get; set; }
        public TimeSpan AverageResponseTimeWrong { get; set; }

        public Game()
        {
            ButtonPresses = new List<ButtonPress>();
        }
    }
}
