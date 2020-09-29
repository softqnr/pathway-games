using PathwayGames.Models;
using PathwayGames.Sensors;
using PathwayGames.Services.Engangement;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.User;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        private const int PlotUpdateInterval = 20;
        private readonly Timer timer;
        private readonly Stopwatch watch = new Stopwatch();
        //Services
        private readonly ISensorLogWriterService _sensorLowWriterService;
        private readonly IUserService _userService;
        private readonly IEngangementService _engangementService;

        public UserGameSettings _userSettings;

        public UserGameSettings UserSettings
        {
            get => _userSettings;
            private set => SetProperty(ref _userSettings, value);
        }

        public ICommand EyeGazeChangedCommand
        {
            get
            {
                return new Command<FaceAnchorChangedEventArgs>((e) =>
                {
                    //_sensorLowWriterService.WriteToLog(e.Reading.ToString());
                    // Invoke engangement service
                    //_engangementService
                });
            }
        }

        public SensorsViewModel (ISensorLogWriterService sensorLogWriterService,
            IUserService userService,
            IEngangementService engangementService)
        {
            _sensorLowWriterService = sensorLogWriterService;
            _userService = userService;
            _engangementService = engangementService;

            Title = "Live";
            
            //StartSensorRecording();
            CreatePlot();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            UserSettings = await _userService.GetUserSettings(App.SelectedUser.Id);
        }

        private void CreatePlot()
        {
 
        }

        private void Update()
        {

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

            //_sensorLowWriterService.Cancel();
        }
    }
}
