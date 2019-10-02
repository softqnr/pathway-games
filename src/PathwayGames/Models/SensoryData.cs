using System.Collections.Generic;

namespace PathwayGames.Models
{
    public class SensoryData
    {
        public List<ButtonPress> ButtonPresses { get; set; }

        public SensoryData()
        {
            ButtonPresses = new List<ButtonPress>();
        }
    }
}
