using PathwayGames.Models;
using PathwayGames.Services.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SessionDataViewModel : ViewModelBase
    {
        readonly IUserService _userService;
        private IList<UserGameSession> _gameSessions;

        public IList<UserGameSession> GameSessions
        {
            get => _gameSessions;
            set => SetProperty(ref _gameSessions, value);
        }

        public ICommand GameDataCommand
        {
            get
            {
                return new Command(async (s) =>
                {
                    await ShareGameData((string)s);
                });
            }
        }

        public ICommand ExportDataCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await ExportGameData();
                });
            }
        }

        private async Task ShareGameData(string gameDataFile)
        {
            var file = Path.Combine(App.LocalStorageDirectory, gameDataFile);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Pathway+ Games - Test results",
                File = new ShareFile(file)
            });
        }

        public async Task ExportGameData()
        {
            await Task.FromResult(true);
        }

        public SessionDataViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            DialogService.ShowLoading("Loading …");
            if (navigationData == null)
            {
                Title = "Session data - " + App.SelectedUser.UserName;
                // Show current selected users game sessions
                GameSessions = await _userService.GetUserGameSessions(App.SelectedUser.Id);
            } else {
                User user = navigationData as User;
                Title = "Session data - " + user.UserName;
                // Show users game sessions
                GameSessions = await _userService.GetUserGameSessions(user.Id);
            }
            DialogService.HideLoading();
        }
    }
}
