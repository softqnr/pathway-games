using PathwayGames.Infrastructure.Keyboard;
using PathwayGames.Models;
using PathwayGames.Services.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private IUserService _userService;

        private UserGameSettings _userSettings;
        private SeekGridOption _selectedSeekGridOption = new SeekGridOption();
        private IList<SeekGridOption> _seekGridOptions;

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

        public ICommand SaveCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OnSave();
                });
            }
        }

        public SettingsViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData == null)
            {
                Title = "Settings - " + App.SelectedUser.UserName;
                // Show current selected users game sessionss
                UserSettings = await _userService.GetUserSettings(App.SelectedUser.Id);
                SeekGridOptions = await _userService.GetSeekGridOptionsByIdiom(DeviceInfo.Idiom.ToString());
                // Set selected option
                if (UserSettings.SeekGridOptions != null)
                {
                    SelectedSeekGridOption = SeekGridOptions.FirstOrDefault(x => x.Id == UserSettings.SeekGridOptions.Id);
                } 
            }
        }

        private async Task OnSave()
        {
            _userSettings.SeekGridOptions = SelectedSeekGridOption;
            await _userService.UpdateUserSettings(_userSettings);
            App.SelectedUser.UserSettings = _userSettings;
            DependencyService.Get<IKeyboardService>().HideKeyboard();
            DialogService.ShowToast("Settings saved.");
        }
    }
}
