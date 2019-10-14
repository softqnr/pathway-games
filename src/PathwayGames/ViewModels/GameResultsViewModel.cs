using PathwayGames.Models;
using PathwayGames.Services.Excel;
using PathwayGames.Infrastructure.Share;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.IO;

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

        public ICommand GameDataCommand
        {
            get
            {
                return new Command(() =>
                {
                    ShareGameData();
                });
            }
        }

        public ICommand SensorDataCommand
        {
            get
            {
                return new Command(() =>
                {
                    ShareSensorData();
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

        private void ShareGameData()
        {
            string filePath = Path.Combine(App.LocalStorageDirectory, _game.GameDataFile);
            DependencyService.Get<IShare>().ShareFile("Share game data", "Share game data", filePath);
        }

        private void ShareSensorData()
        {
            string filePath = Path.Combine(App.LocalStorageDirectory, _game.SensorDataFile);
            DependencyService.Get<IShare>().ShareFile("Share sensor data", "Share sensor data", filePath);
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
