namespace PathwayGames.Controls
{
    public interface ISensor
    {
        bool RecordingEnabled { get; set; }
        void OnEyeGazeChanged(EyeGazeChangedEventArgs e);
    }
}
