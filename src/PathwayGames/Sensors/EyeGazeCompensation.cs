namespace PathwayGames.Sensors
{
    public class EyeGazeCompensation
    {
        public float WidthCompensation { get; set; }

        public float HeightCompensation { get; set; }

        public EyeGazeCompensation(float widthCompensation, float heightCompensation)
        {
            WidthCompensation = widthCompensation;
            HeightCompensation = heightCompensation;
        }
    }
}
