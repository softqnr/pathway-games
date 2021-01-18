using PathwayGames.Infrastructure.Device;
using PathwayGames.Infrastructure.Keyboard;
using PathwayGames.Models;
using PathwayGames.Sensors;
using PathwayGames.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IDeviceHelper _deviceHelper;

        private UserGameSettings _userSettings;
        private SeekGridOption _selectedSeekGridOption = new SeekGridOption();
        private IList<SeekGridOption> _seekGridOptions;
        private string _visualizationSettings;
        private EyeGazeCompensation _compensantion;
        private int _ppi;

        public SeekGridOption SelectedSeekGridOption
        {
            get => _selectedSeekGridOption;
            set => SetProperty(ref _selectedSeekGridOption, value);
        }

        public UserGameSettings UserSettings
        {
            get => _userSettings;
            set => SetProperty(ref _userSettings, value);
        }

        public IList<SeekGridOption> SeekGridOptions
        {
            get => _seekGridOptions;
            set => SetProperty(ref _seekGridOptions, value);
        }

        public string VisualizationSettings
        {
            get => _visualizationSettings;
            set => SetProperty(ref _visualizationSettings, value);
        }

        public string[] Tolerances { get; } = Enum.GetNames(typeof(Models.Enums.Tolerance));

        public ICommand SaveSettingsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OnSaveSettings();
                });
            }
        }

        public ICommand ResetVisualizationDefaultsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OnResetVisualizationDefaults();
                });
            }
        }

        public SettingsViewModel(IUserService userService)
        {
            _userService = userService;
            _deviceHelper = DependencyService.Get<IDeviceHelper>();

            _ppi = _deviceHelper.MachineNameToPPI(DeviceInfo.Model);
            _compensantion = _deviceHelper.MachineNameToEyeGazeCompensation(DeviceInfo.Model);
        }

        public override async Task InitializeAsync(object navigationData)
        {
            Title = Resources["TitleSettings"] + " - " + App.SelectedUser.UserName;
            VisualizationSettings = $"({DeviceInfo.Model}, {_ppi}PPI, [{_compensantion.WidthCompensation}-{_compensantion.HeightCompensation}])";
            // Show current selected users game sessionss
            UserSettings = await _userService.GetUserSettings(App.SelectedUser.Id);
            SeekGridOptions = await _userService.GetSeekGridOptionsByIdiom(DeviceInfo.Idiom.ToString());
            // Set selected option
            if (UserSettings.SeekGridOptions != null)
            {
                SelectedSeekGridOption = SeekGridOptions.FirstOrDefault(x => x.Id == UserSettings.SeekGridOptions.Id);
            } 
        }

        private async Task OnSaveSettings()
        {
            _userSettings.SeekGridOptions = SelectedSeekGridOption;
            await _userService.UpdateUserSettings(_userSettings);
            App.SelectedUser.UserSettings = _userSettings;
            DependencyService.Get<IKeyboardService>().HideKeyboard();
            DialogService.ShowToast(Resources["TitleSettingsSaved"]);
        }

        private async Task OnResetVisualizationDefaults()
        {
            bool confirmed = await DialogService.ShowConfirmAsync(Resources["TitleCannotUndoThisAction"],
                Resources["PromptResetDefaultSettings"], Resources["Ok"], Resources["Cancel"]);
            if (confirmed)
            {
                _userSettings.ScreenPPI = _ppi;
                _userSettings.VisualizationWidthCompensation = _compensantion.WidthCompensation;
                _userSettings.VisualizationHeightCompensation = _compensantion.HeightCompensation;
                this.OnPropertyChanged(nameof(UserSettings));
            }
        }
    }
}
