using System.Collections.Generic;

namespace PathwayGames.Models
{
    public class SensoryData
    {
        public List<ButtonPress> ButtonPresses { get; set; }

        public List<FaceAnchorData> FaceAnchorData { get; set; }

        public SensoryData()
        {
            ButtonPresses = new List<ButtonPress>();
            FaceAnchorData = new List<FaceAnchorData>();
        }
    }
}
