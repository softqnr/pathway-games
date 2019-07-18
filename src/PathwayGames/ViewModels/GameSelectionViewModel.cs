using PathwayGames.Models.Enums;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class GameSelectionViewModel : ViewModelBase
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

        public ICommand TypeAX
        {
            get
            {
                return new Command(async () =>
                {
                    await NavigationService.NavigateToAsync<GameViewModel>(GameType.TypeAX);
                });
            }
        }

    }
}
