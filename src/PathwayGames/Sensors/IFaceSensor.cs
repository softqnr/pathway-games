namespace PathwayGames.Sensors
{
    public interface IFaceSensor : ISensor<FaceAnchorChangedEventArgs>
    {
        bool CameraPreviewEnabled { get; set; }

        bool EyeGazeVisualizationEnabled { get; set; }

        int ScreenPPI { get; set; }

        float WidthCompensation { get; set; }

        float HeightCompensation { get; set; }
    }
}
