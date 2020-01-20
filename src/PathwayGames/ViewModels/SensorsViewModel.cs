using PathwayGames.Sensors;
using PathwayGames.Services.Engangement;
using PathwayGames.Services.Sensors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        //Services
        private readonly ISensorLogWriterService _sensorLowWriterService;
        private readonly IEngangementService _engangementService;

        public ICommand EyeGazeChangedCommand
        {
            get
            {
                return new Command<FaceAnchorChangedEventArgs>((e) =>
                {
                    _sensorLowWriterService.WriteToLog(e.Reading.ToString());
                    // Invoke engangement service
                    //_engangementService
                });
            }
        }

        public SensorsViewModel (ISensorLogWriterService sensorLogWriterService,
            IEngangementService engangementService)
        {
            _sensorLowWriterService = sensorLogWriterService;
            _engangementService = engangementService;

            Title = "Live";

            StartSensorRecording();
        }

        private void StartSensorRecording()
        {
            _sensorLowWriterService.LogPrefix = "\"FaceAnchorData\": [";
            _sensorLowWriterService.LogSuffix = "] ";

            _sensorLowWriterService.Start($"sensor_{Guid.NewGuid().ToString()}.json", ",");
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            _sensorLowWriterService.Cancel();
        }
    }
}
