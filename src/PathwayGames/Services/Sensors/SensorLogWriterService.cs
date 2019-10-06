﻿using PathwayGames.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace PathwayGames.Services.Sensors
{
    public class SensorLogWriterService : ISensorLogWriterService
    {
        private static Queue<string> LogQueue;

        private int FlushAtAge = 2;

        private int FlushAtQty = 500;

        private DateTime FlushedAt;

        public string MessangingCenterMessage { get; private set; }

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
        
        public void Start(string logFile, string messangingCenterMessage)
        {
            Start(logFile, messangingCenterMessage, "");
        }
        
        public void Start(string logFile, string messangingCenterMessage, string logItemSeparator)
        {
            LogPath = App.LocalStorageDirectory;
            MessangingCenterMessage = messangingCenterMessage;
            LogFile = logFile;
            LogItemSeparator = logItemSeparator;

            IsMonitoring = true;

            if (LogPrefix != "")
            {
                WriteToLog(LogPrefix);
            }

            MessagingCenter.Subscribe<object, FaceAnchorData>( this, MessangingCenterMessage, (s, d) => {
                WriteToLog(d.ToString() + LogItemSeparator);
            });
        }

        private void WriteToLog(string reading)
        {
            lock (LogQueue)
            {
                // Create log
                //Log log = new Log(message);
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
            MessagingCenter.Unsubscribe<object, FaceAnchorData>(this, MessangingCenterMessage);
            IsMonitoring = false;

            ForceFlush();

            if (LogItemSeparator != "")
            {
                RemoveLastSeperatorAndAppendSuffix(LogSuffix);
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
                using (var writer = new StreamWriter(LogFilePath, true, System.Text.Encoding.UTF8))
                {
                    while (LogQueue.Count > 0) {
                        // Get entry to log
                        string reading = LogQueue.Dequeue();
                        // Log to file
                        writer.WriteLine(reading);
                    }
                }
            }
        }

        private void RemoveLastSeperatorAndAppendSuffix(string logSuffix)
        {
            using (FileStream fs = new FileStream(LogFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                int seperatorByteCount = System.Text.Encoding.UTF8.GetByteCount(LogItemSeparator + Environment.NewLine);
                fs.Position = fs.Seek(seperatorByteCount * -1, SeekOrigin.End);

                if (logSuffix != "")
                {
                    foreach (byte chr in System.Text.Encoding.UTF8.GetBytes(logSuffix))
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
