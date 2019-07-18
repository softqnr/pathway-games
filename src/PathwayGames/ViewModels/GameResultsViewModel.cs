using PathwayGames.Models;
using PathwayGames.Services.Excel;
using PathwayGames.Services.Share;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class GameResultsViewModel : ViewModelBase
    {
        private Game _game;
        private IExcelService _excelService;

        public Game Game
        {
            get => _game;
            set => SetProperty(ref _game, value);
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

        public GameResultsViewModel(IExcelService excelService)
        {
            _excelService = excelService;
        }
            
        private async Task GenerateAndShareExcelAsync()
        {
            DialogService.ShowLoading("Generating results …");
            string fileName = await _excelService.ExportAsync(_game);
            DialogService.HideLoading();
            DependencyService.Get<IShare>().ShareFile("Share results", "Share results", fileName);
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null)
            {
                Game = navigationData as Game;
            }
            await Task.FromResult(true);
        }
    }
}
