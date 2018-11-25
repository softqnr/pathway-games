using PathwayGames.Services.Navigation;
using PathwayGames.Views;
using PathwayGames.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Unity;
using Unity.ServiceLocation;
using Unity.Lifetime;
using CommonServiceLocator;
using PathwayGames.Services.Slides;
using PathwayGames.Services.Sound;
using PathwayGames.Services.Sensors;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PathwayGames
{
    public partial class App : Application
    {
        public static UnityContainer Container { get; private set; }
        public readonly static INavigationService NavigationService = new NavigationService();
        public App()
        {
            InitializeComponent();

            // Init DI
            InitializeDI();

            MainPage = new MasterDetailView();
        }

        private async Task InitializeNavigation()
        {
            NavigationService.Configure(typeof(MasterViewModel), typeof(MasterView));
            NavigationService.Configure(typeof(GameSelectionViewModel), typeof(GameSelectionView));
            NavigationService.Configure(typeof(GameViewModel), typeof(GameView));
            NavigationService.Configure(typeof(SettingsViewModel), typeof(SettingsView));
            NavigationService.Configure(typeof(ThankYouViewModel), typeof(ThankYouView));
            NavigationService.Configure(typeof(SessionDataViewModel), typeof(SessionDataView));
            NavigationService.Configure(typeof(SensorsViewModel), typeof(SensorsView));
            NavigationService.Configure(typeof(UsersViewModel), typeof(UsersView));
            await NavigationService.InitializeAsync();
        }

        private void InitializeDI()
        {
            Container = new UnityContainer();

            // Services
            Container.RegisterInstance(NavigationService, new ContainerControlledLifetimeManager());
            Container.RegisterType<ISoundService, SoundService>();
            //Container.RegisterType<IDialogService, DialogService>();

            // Data services
            Container.RegisterType<ISlidesService, SlidesService>();
            Container.RegisterType<ISensorsService, SensorsService>();

            // View models
            Container.RegisterType<MasterViewModel>();
            Container.RegisterType<MainViewModel>();
            Container.RegisterType<GameSelectionViewModel>();
            Container.RegisterType<GameViewModel>();
            Container.RegisterType<SettingsViewModel>();
            Container.RegisterType<ThankYouViewModel>();
            Container.RegisterType<SessionDataViewModel>();
            Container.RegisterType<SensorsViewModel>();
            Container.RegisterType<UsersViewModel>();

            // Set as service locator provider
            var unityServiceLocator = new UnityServiceLocator(Container);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
        }


        protected override async void OnStart()
        {
            // Handle when your app starts
            // Nav service configuration
            await InitializeNavigation();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
