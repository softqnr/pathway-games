using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using PathwayGames.Services.Engangement;
using PathwayGames.Infrastructure.Timer;
using PathwayGames.Services.User;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using PathwayGames.Infrastructure.Device;
using Xamarin.Essentials;

namespace PathwayGames.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        TimeSpan LightUpdateTimespan = TimeSpan.FromMilliseconds(10);
        //Services
        private readonly IUserService _userService;
        private readonly IEngagementService _engagementService;

        private bool _visualizationEnabled;
        private bool _recordingEnabled;
        private CancelableTimer _timer;
        private Brush _lightColor = new SolidColorBrush(Color.White);
        private UserGameSettings _userSettings;

        public UserGameSettings UserSettings
        {
            get => _userSettings;
            private set => SetProperty(ref _userSettings, value);
        }

        public bool VisualizationEnabled
        {
            get => _visualizationEnabled;
            private set => SetProperty(ref _visualizationEnabled, value);
        }

        public bool RecordingEnabled
        {
            get => _recordingEnabled;
            private set => SetProperty(ref _recordingEnabled, value);
        }

        public Brush LightColor
        {
            get => _lightColor;
            private set => SetProperty(ref _lightColor, value);
        }

        public ICommand EyeGazeChangedCommand
        {
            get
            {
                return new Command<FaceAnchorChangedEventArgs>((e) =>
                {
                    // Invoke engangement service
                    _engagementService.UpdateEngagement(e.Reading);
                });
            }
        }

        public ICommand FaceSensorTappedCommand
        {
            get
            {
                return new Command(() =>
                {
                    // Tongel visualization
                    VisualizationEnabled = !VisualizationEnabled;
                });
            }
        }

        public SensorsViewModel (IUserService userService)
        {
            _userService = userService;
            _engagementService = DependencyService.Get<IEngagementService>();
            
            Title = Resources["TitleLive"];
        }

        public override async Task InitializeAsync(object navigationData)
        {
            UserSettings = await _userService.GetUserSettings(App.SelectedUser.Id);
            VisualizationEnabled = UserSettings.EyeGazeVisualisation;

            // Start ML Session
            var ppi = DependencyService.Get<IDeviceHelper>().MachineNameToPPI(DeviceInfo.Model);
            _engagementService.StartSession(ppi, UserSettings.LiveViewSensitivity);

            // Start sensor read
            RecordingEnabled = true;

            // Timer init
            _timer = new CancelableTimer(LightUpdateTimespan, UpdateLightColor);
            _timer.Start();
        }

        private void UpdateLightColor()
        {
            Device.BeginInvokeOnMainThread(() => {
                LightColor = new SolidColorBrush(_engagementService.GetEngagementColor(
                    (Tolerance)Enum.Parse(typeof(Tolerance), UserSettings.LiveViewTolerance)));
            });
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_timer != null)
            {
                _timer.Stop();
            }
        }
    }
}
