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

namespace PathwayGames.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        TimeSpan LightUpdateTimespan = TimeSpan.FromMilliseconds(10);
        //Services
        private readonly IUserService _userService;
        private readonly IEngangementService _engangementService;

        private bool _recordingEnabled;
        private CancelableTimer _timer;
        private Brush _lightColor = new SolidColorBrush(Color.White);
        private UserGameSettings _userSettings;

        private LiveUserState _liveUserState;

        public UserGameSettings UserSettings
        {
            get => _userSettings;
            private set => SetProperty(ref _userSettings, value);
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
                    // here will send sensor data for your algorithm calculation
                    _engangementService.CalculateEngangement(UserSettings.LiveViewSensitivity, e.Reading);
                });
            }
        }

        public SensorsViewModel (IUserService userService, IEngangementService engangementService)
        {
            _userService = userService;
            _engangementService = engangementService;

            _liveUserState = new LiveUserState();

            Title = Resources.AppResources.TitleLive;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            UserSettings = await _userService.GetUserSettings(App.SelectedUser.Id);

            // Start sensor read
            RecordingEnabled = true;

            // Timer init
            _timer = new CancelableTimer(LightUpdateTimespan, UpdateLightColor);
            _timer.Start();

            _liveUserState.Sensitivity = UserSettings.LiveViewSensitivity;
            _liveUserState.StartSession();
        }

        private void UpdateLightColor()
        {
            Device.BeginInvokeOnMainThread(() => {
                LightColor = new SolidColorBrush(_engangementService.GetEngangementColor(
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
