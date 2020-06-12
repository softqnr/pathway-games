using Newtonsoft.Json;

namespace PathwayGames.Models
{
    public class Outcome
    {
        public int Score { get; set; }

        [JsonIgnore]
        public double ScorePercentage { get; set; }

        public double AverageResponseTime { get; set; }

        public double AverageResponseTimeCorrect { get; set; }

        public double AverageResponseTimeWrong { get; set; }

        public ConfusionMatrix ConfusionMatrix { get; set; } = new ConfusionMatrix();

        public Outcome()
        {

        }
    }
}
