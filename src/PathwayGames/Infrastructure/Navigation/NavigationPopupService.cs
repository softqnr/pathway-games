using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using PathwayGames.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PathwayGames.Infrastructure.Navigation
{
    public partial class NavigationService : INavigationService
    {

        public Task NavigateToPopupAsync<TViewModel>(bool animate) where TViewModel : ViewModelBase
        {
            return NavigateToPopupAsync<TViewModel>(null, animate);
        }

        public async Task NavigateToPopupAsync<TViewModel>(object parameter, bool animate) where TViewModel : ViewModelBase
        {
            var page = CreateAndBindPage(typeof(TViewModel), parameter);
            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
            
            if (page is PopupPage)
            {
                await PopupNavigation.Instance.PushAsync(page as PopupPage, animate);
            }
            else if (page is Page)
            {
                await CurrentNavigation.PushModalAsync(new NavigationPage(page));
            }
            else
            {
                throw new InvalidNavigationException($"The type ${typeof(TViewModel)} its not a Page/PopupPage type");
            }
        }

        public async Task PopAsync(bool animate)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                await PopupNavigation.Instance.PopAsync(animate);
            }
            else
            {
                await CurrentNavigation.PopModalAsync(animate);
            }
        }
    }
}
