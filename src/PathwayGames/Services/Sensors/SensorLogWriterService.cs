using PathwayGames.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PathwayGames.Services.Sensors
{
    public class SensorLogWriterService : ISensorLogWriterService
    {
        private static Queue<string> LogQueue;

        private int FlushAtAge = 2;

        private int FlushAtQty = 500;

        private DateTime FlushedAt;

        public long LogItemCount { get; private set; }

        public string LogItemSeparator { get; private set; }

        public string LogPrefix { get; set; }

        public string LogSuffix { get; set; }

        public string LogPath { get; private set; }

        public string LogFile { get; private set; }

        public string LogFilePath { get => Path.Combine(LogPath, LogFile); }

        public bool IsMonitoring { get; private set; }

        public SensorLogWriterService()
        {
            //Instance = new LogWriter();
            LogQueue = new Queue<string>();
            FlushedAt = DateTime.Now;
        }
        
        public void Start(string logFile)
        {
            Start(logFile, "");
        }
        
        public void Start(string logFile, string logItemSeparator)
        {
            LogPath = App.LocalStorageDirectory;
            LogFile = logFile;
            LogItemSeparator = logItemSeparator;

            IsMonitoring = true;

            if (LogPrefix != "")
            {
                WriteRawTextToLog(LogPrefix);
            }
        }

        public void WriteToLog(string reading)
        {
            WriteToLog(reading, true);
        }

        public void WriteToLog(string reading, bool writeSeparator = true)
        {
            lock (LogQueue)
            {
                // Create log
                //Log log = new Log(message);
                if (writeSeparator)
                {
                    reading += LogItemSeparator;
                }

                LogQueue.Enqueue(reading);

                // Check if should flush
                if (LogQueue.Count >= FlushAtQty || CheckTimeToFlush())
                {
                    FlushLogToFile();
                }
            }
        }

        public void Stop()
        {
            IsMonitoring = false;

            ForceFlush();

            if (LogItemSeparator != "" && LogItemCount > 0)
            {
                RemoveLastSeperator();
            }

            if (LogSuffix != "")
            {
                WriteRawTextToLog(LogSuffix);
            }
        }

        public void Cancel()
        {
            if (IsMonitoring)
            {
                IsMonitoring = false;
                LogQueue.Clear();
                if (File.Exists(LogFilePath))
                    File.Delete(LogFilePath);

                LogFile = "";
            }
        }

        public void ForceFlush()
        {
            FlushLogToFile();
        }

        private bool CheckTimeToFlush()
        {
            TimeSpan time = DateTime.Now - FlushedAt;
            if (time.TotalSeconds >= FlushAtAge)
            {
                FlushedAt = DateTime.Now;
                return true;
            }
            return false;
        }

        private void FlushLogToFile()
        {
            if (LogQueue.Count > 0) {
                using (var writer = new StreamWriter(LogFilePath, true, new UTF8Encoding(false)))
                {
                    while (LogQueue.Count > 0) {
                        // Get entry to log
                        string reading = LogQueue.Dequeue();
                        // Log to file
                        writer.WriteLine(reading);

                        LogItemCount++;
                    }
                }
            }
        }

        private void RemoveLastSeperator()
        {
            using (FileStream fs = new FileStream(LogFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                int seperatorByteCount = Encoding.UTF8.GetByteCount(LogItemSeparator + Environment.NewLine);
                // Overwrite separator
                fs.Position = fs.Seek(seperatorByteCount * -1, SeekOrigin.End);

                fs.SetLength(fs.Position);
                fs.Close();
            }
        }

        private void WriteRawTextToLog(string text)
        {
            using (FileStream fs = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write))
            {
                // Move to end
                fs.Position = fs.Seek(0, SeekOrigin.End);

                if (text != "")
                {
                    foreach (byte chr in Encoding.UTF8.GetBytes(text))
                    {
                        fs.WriteByte(chr);
                    }
                }

                fs.SetLength(fs.Position);
                fs.Close();
            }
        }

    }
}
