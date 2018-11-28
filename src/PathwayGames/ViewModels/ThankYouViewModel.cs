using PathwayGames.Models;
using PathwayGames.Services.Excel;
using PathwayGames.Services.Share;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class ThankYouViewModel : ViewModelBase
    {
        private Game _game;
        private IExcelService _excelService;

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
        public ThankYouViewModel(IExcelService excelService)
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
                _game = navigationData as Game;
            }
        }
    }
}
