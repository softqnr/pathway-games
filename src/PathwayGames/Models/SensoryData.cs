using PathwayGames.Sensors;
using System.Collections.Generic;

namespace PathwayGames.Models
{
    public class SensoryData
    {
        public List<ButtonPress> ButtonPresses { get; set; } = new List<ButtonPress>();

        public List<FaceAnchorReading> FaceAnchorData { get; set; } = new List<FaceAnchorReading>();

        public SensoryData()
        {

        }
    }
}
