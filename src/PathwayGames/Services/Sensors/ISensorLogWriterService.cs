﻿namespace PathwayGames.Services.Sensors
{
    public interface ISensorLogWriterService
    {
        string LogItemSeparator { get; }

        string LogPath { get; }

        string LogFilePath { get; }

        string LogPrefix { get; set; }

        string LogSuffix { get; set; }

        bool IsMonitoring { get; }

        void Start(string logFile);

        void Start(string logFile, string separetor);

        void WriteToLog(string reading);

        void Stop();
    }
}
