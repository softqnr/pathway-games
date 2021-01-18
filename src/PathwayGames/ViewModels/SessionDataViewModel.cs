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
            if (!IsBusy)
            {
                IsBusy = true;
                var file = Path.Combine(App.LocalStorageDirectory, gameDataFile);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = $"{Resources["TitleTestResults"]} - {Resources["TitleTestResults"]}",
                    File = new ShareFile(file),
                    PresentationSourceBounds = Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Tablet
                                            ? new System.Drawing.Rectangle(0, 20, 0, 0)
                                            : System.Drawing.Rectangle.Empty
                });

                IsBusy = false;
            }
        }

        public async Task ExportAllUserGameData()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                DialogService.ShowLoading(Resources["TitleGeneratingPackage"]);
                string fileName = await _userService.PackAllUserGameSessions();
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = Resources["TitleShareData"],
                    File = new ShareFile(fileName),
                    PresentationSourceBounds = Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Tablet
                                            ? new System.Drawing.Rectangle(0, 20, 0, 0)
                                            : System.Drawing.Rectangle.Empty
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
                DialogService.ShowLoading(Resources["TitleGeneratingPackage"]);
                string fileName = await _userService.PackUserGameSessions(_user.Id);
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = Resources["TitleShareData"],
                    File = new ShareFile(fileName),
                    PresentationSourceBounds = Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Tablet
                                            ? new System.Drawing.Rectangle(0, 20, 0, 0)
                                            : System.Drawing.Rectangle.Empty
                });
                DialogService.HideLoading();
                IsBusy = false;
            }
        }

        public async Task DeleteGameSession(UserGameSession userGameSession)
        {
            bool confirmed = await DialogService.ShowConfirmAsync(Resources["TitleCannotUndoThisAction"],
                Resources["PromptDeleteSessionData"], Resources["Ok"], Resources["Cancel"]);
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
            DialogService.ShowLoading(Resources["TitleLoading"]);
            if (navigationData == null)
            {
                _user = App.SelectedUser;
                Title = $"{Resources["TitleSessionData"]} - {App.SelectedUser.UserName}";
            } else {
                _user = navigationData as User;
                Title = $"Resources.AppResources.TitleSessionData - {_user.UserName}";
            }
            GameSessions = new ObservableCollection<UserGameSession>(await _userService.GetUserGameSessions(_user.Id));
            DialogService.HideLoading();
        }
    }
}
