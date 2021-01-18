using PathwayGames.Models;
using PathwayGames.Services.Excel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.IO;
using Xamarin.Essentials;
using PathwayGames.Services.User;
using PathwayGames.Infrastructure.Navigation;

namespace PathwayGames.ViewModels
{
    public class GameResultsViewModel : ViewModelBase
    {
        private Game _game;
        private UserGameSession _userGameSession;

        private readonly IExcelService _excelService;
        private readonly IUserService _userService;

        public Game Game
        {
            get => _game;
            set => SetProperty(ref _game, value);
        }

        public ICommand GameDataCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await ShareGameData();
                });
            }
        }

        public ICommand ExportButtonCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await GenerateAndShareExcelAsync();
                });
            }
        }

        public ICommand DeleteSessionCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await DeleteSession();
                });
            }
        }

        public GameResultsViewModel(IExcelService excelService, IUserService userService)
        {
            _excelService = excelService;
            _userService = userService;
        }
            
        private async Task GenerateAndShareExcelAsync()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                DialogService.ShowLoading(Resources["TitleGeneratingResults"]);
                string fileName = await _excelService.ExportAsync(_game);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = Resources["TitleShareResults"],
                    File = new ShareFile(fileName),
                    PresentationSourceBounds = Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Tablet
                                                ? new System.Drawing.Rectangle(0, 20, 0, 0)
                                                : System.Drawing.Rectangle.Empty
                });

                DialogService.HideLoading();
                IsBusy = false;
            }
        }

        private async Task ShareGameData()
        {
            var file = Path.Combine(App.LocalStorageDirectory, _game.GameDataFile);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = $"{Resources["ApplicationName"]} - {Resources["TitleTestResults"]}",
                File = new ShareFile(file),
                PresentationSourceBounds = Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Tablet
                                            ? new System.Drawing.Rectangle(0, 20, 0, 0)
                                            : System.Drawing.Rectangle.Empty
            });
        }

        private async Task DeleteSession()
        {
            bool confirmed = await DialogService.ShowConfirmAsync(Resources["TitleCannotUndoThisAction"],
                Resources["PromptDeleteSessionData"], Resources["Ok"], Resources["Cancel"]);
            if (confirmed)
            {
                await _userService.DeleteGameSession(_userGameSession);
                await NavigationService.NavigateBackAsync();
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null)
            {
                var p = navigationData as NavigationParameters;
                Game = (Game)p.GetValue("game");
                _userGameSession = (UserGameSession)p.GetValue("game_session");
            }
            await Task.FromResult(true);
        }
    }
}
