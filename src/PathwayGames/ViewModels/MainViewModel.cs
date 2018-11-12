using PathwayGames.Models.Enums;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand TypeX {
            get
            {
                return new Command(async () =>
                {
                    await NavigationService.NavigateToAsync<GameViewModel>(GameType.TypeX);
                });
            }
        }

    }
}
