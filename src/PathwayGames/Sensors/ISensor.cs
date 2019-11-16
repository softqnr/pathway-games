namespace PathwayGames.Sensors
{
    public interface ISensor<T>
    {
        //bool IsAvailable { get; }
        bool RecordingEnabled { get; set; }

        void OnReadingTaken(T e);
    }
}
