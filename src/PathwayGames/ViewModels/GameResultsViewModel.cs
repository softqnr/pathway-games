using PathwayGames.Models;
using PathwayGames.Services.Excel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.IO;
using Xamarin.Essentials;

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
                return new Command(async () =>
                {
                    await ShareGameData();
                });
            }
        }

        public ICommand SensorDataCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await ShareSensorData();
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

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share results",
                File = new ShareFile(fileName)
            });

            DialogService.HideLoading();
        }

        private async Task ShareGameData()
        {
            var message = new EmailMessage
            {
                Subject = "Pathway+ Games - Test results",
                Body = "",
            };

            message.Attachments.Add(new EmailAttachment(Path.Combine(App.LocalStorageDirectory, _game.GameDataFile)));

            await Email.ComposeAsync(message);
        }

        private async Task ShareSensorData()
        {
            var message = new EmailMessage
            {
                Subject = "Pathway+ Games - Sensor data",
                Body = "",
            };

            message.Attachments.Add(new EmailAttachment(Path.Combine(App.LocalStorageDirectory, _game.SensorDataFile)));

            await Email.ComposeAsync(message);
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
