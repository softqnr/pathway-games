using PathwayGames.Models;
using PathwayGames.Services.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private IUserService _userService;
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private UserSettings _userSettings;

        public UserSettings UserSettings
        {
            get => _userSettings;
            set => SetProperty(ref _userSettings, value);
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
            }
        }

        private async Task OnSave()
        {
            //_userService.SaveUserSettings(App.SelectedUser.Id);
            Task.FromResult(true);
        }
    }
}
