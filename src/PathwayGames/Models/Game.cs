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
        public double Score { get; set; }
    }
}
