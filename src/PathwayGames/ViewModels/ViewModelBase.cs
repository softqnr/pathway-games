using CommonServiceLocator;
using PathwayGames.Infrastructure.Dialog;
using PathwayGames.Infrastructure.Navigation;
using System.Threading.Tasks;

namespace PathwayGames.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        protected readonly INavigationService NavigationService;
        protected readonly IDialogService DialogService;

        public ViewModelBase()
        {
            DialogService = ServiceLocator.Current.GetInstance<IDialogService>();
            NavigationService = ServiceLocator.Current.GetInstance<INavigationService>();
        }

        string title = string.Empty;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

        public virtual void OnBackButtonPressed()
        {

        }

        public virtual void OnAppearing()
        {

        }

        public virtual void OnDisappearing()
        {

        }
    }
}
