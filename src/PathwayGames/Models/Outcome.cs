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

        public double AverageResponseTime { get; set; }

        public double AverageResponseTimeCorrect { get; set; }

        public double AverageResponseTimeWrong { get; set; }

        public Outcome()
        {
            //ConfusionMatrix = new ConfusionMatrix();
        }
    }
}
