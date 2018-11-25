using System;
using System.Collections.Generic;
using PathwayGames.Models;

namespace PathwayGames.Services.Sensors
{
    public class SensorsService : ISensorsService
    {
        public List<Sensor> GetSensors()
        {
            return new List<Sensor> {
                new Sensor() {Name = "Eye Gaze", Enabled = true},
                new Sensor() {Name = "EEG", Enabled = true},
                new Sensor() {Name = "Muse", Enabled = true }
            };
        }

        public void ChangeRecordingLabels()
        {
            
        }

        public void StartRecording()
        {
            
        }

        public void StopRecording()
        {
           
        }
    }
}
