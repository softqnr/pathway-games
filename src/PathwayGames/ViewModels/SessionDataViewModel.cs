using PathwayGames.Models;
using PathwayGames.Services.User;
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
        private ObservableCollection<UserGameSession> _gameSessions;
        private Models.User _user;

        public ObservableCollection<UserGameSession> GameSessions
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

        public ICommand ExportUserDataCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await ExportUserGameData();
                });
            }
        }

        public ICommand ExportAllUserDataCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await ExportAllUserGameData();
                });
            }
        }

        public ICommand DeleteGameSessionCommand
        {
            get
            {
                return new Command(async (g) =>
                {
                    await DeleteGameSession((UserGameSession)g);
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

        public async Task ExportAllUserGameData()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                DialogService.ShowLoading("Generating package …");
                string fileName = await _userService.PackAllUserGameSessions();
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Share data",
                    File = new ShareFile(fileName)
                });
                DialogService.HideLoading();
                IsBusy = false;
            }
        }

        public async Task ExportUserGameData()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                DialogService.ShowLoading("Generating package …");
                string fileName = await _userService.PackUserGameSessions(_user.Id);
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Share data",
                    File = new ShareFile(fileName)
                });
                DialogService.HideLoading();
                IsBusy = false;
            }
        }

        public async Task DeleteGameSession(UserGameSession userGameSession)
        {
            bool confirmed = await DialogService.ShowConfirmAsync("You cannot undo this action", 
                "Do you want to delete this session data?", "Ok", "Cancel");
            if (confirmed)
            {
                await _userService.DeleteGameSession(userGameSession);
                GameSessions.Remove(userGameSession);
            }
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
                _user = App.SelectedUser;
                Title = "Session data - " + App.SelectedUser.UserName;
            } else {
                _user = navigationData as User;
                Title = "Session data - " + _user.UserName;
            }
            GameSessions = new ObservableCollection<UserGameSession>(await _userService.GetUserGameSessions(_user.Id));
            DialogService.HideLoading();
        }
    }
}
