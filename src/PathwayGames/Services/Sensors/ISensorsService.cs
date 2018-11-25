using PathwayGames.Models;
using System.Collections.Generic;

namespace PathwayGames.Services.Sensors
{
    public interface ISensorsService
    {
        List<Sensor> GetSensors();
        void StartRecording();
        void ChangeRecordingLabels();
        void StopRecording();
    }
}
