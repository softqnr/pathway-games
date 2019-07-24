namespace PathwayGames.Services.Sensors
{
    public interface ISensorLogWriterService
    {
        string FileName { get; }

        bool IsMonitoring { get; }

        void Start();

        void Stop();
    }
}
