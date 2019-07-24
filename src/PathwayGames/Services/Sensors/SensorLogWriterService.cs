using PathwayGames.Controls;
using System;
using System.IO;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames.Services.Sensors
{
    public class SensorLogWriterService : ISensorLogWriterService
    {
        private StreamWriter _sw;

        public string FileName { get; private set; }

        public bool IsMonitoring { get; private set; }

        public void Start()
        {
            string path = FileSystem.AppDataDirectory;
            FileName = "EyeGaze" + Guid.NewGuid().ToString() + ".json";
            string filePath = Path.Combine(path, FileName);

            IsMonitoring = true;

            _sw = new StreamWriter(filePath, true, Encoding.UTF8);
            
            MessagingCenter.Subscribe<object, EyeGazeData>( this, "EyeSensorReading", (s, d) => {
                //Device.BeginInvokeOnMainThread(() =>
                //{
                //sw.WriteLine(d.ToString());
                OnReadingReceived(d);
                //});
            });
        }

        private void OnReadingReceived(EyeGazeData eyeGazeData)
        {
            _sw.WriteLine(eyeGazeData.ToString());
        }

        public void Stop()
        {
            MessagingCenter.Unsubscribe<object, EyeGazeData>(this, "EyeSensorReading");
            IsMonitoring = false;
            FileName = "";
            _sw.Flush();
            _sw.Close();
        }
    }
}
