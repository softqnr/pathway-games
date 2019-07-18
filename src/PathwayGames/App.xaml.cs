using CommonServiceLocator;
using PathwayGames.Infrastructure.Dialog;
using PathwayGames.Infrastructure.Navigation;
using PathwayGames.Infrastructure.Sound;
using PathwayGames.Services.Engangement;
using PathwayGames.Services.Excel;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.ViewModels;
using PathwayGames.Views;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PathwayGames
{
    public partial class App : Application
    {
        public static string UserName = "Quest";
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
            NavigationService.Configure(typeof(GameResultsViewModel), typeof(GameResultsView));
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
            Container.RegisterType<IDialogService, DialogService>();

            // Data services
            Container.RegisterType<ISlidesService, SlidesService>();
            Container.RegisterType<ISensorsService, SensorsService>();
            Container.RegisterType<IEngangementService, EngangementService>();
            Container.RegisterType<IExcelService, ExcelService>();

            // View models
            Container.RegisterType<MasterViewModel>();
            Container.RegisterType<MainViewModel>();
            Container.RegisterType<GameSelectionViewModel>();
            Container.RegisterType<GameViewModel>();
            Container.RegisterType<SettingsViewModel>();
            Container.RegisterType<GameResultsViewModel>();
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
