using Newtonsoft.Json;
using System;

namespace PathwayGames.Models
{
    public class Outcome
    {
        public ConfusionMatrix ConfusionMatrix { get; set; }

        public int Score { get; set; }

        [JsonIgnore]
        public double ScorePercentage { get; set; }

        public TimeSpan AverageResponseTime { get; set; }

        public TimeSpan AverageResponseTimeCorrect { get; set; }

        public TimeSpan AverageResponseTimeWrong { get; set; }

        public Outcome()
        {
            //ConfusionMatrix = new ConfusionMatrix();
        }
    }
}
