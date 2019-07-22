using CommonServiceLocator;
using PathwayGames.Data;
using PathwayGames.Infrastructure.Dialog;
using PathwayGames.Infrastructure.File;
using PathwayGames.Infrastructure.Navigation;
using PathwayGames.Infrastructure.Sound;
using PathwayGames.Models;
using PathwayGames.Services.Engangement;
using PathwayGames.Services.Excel;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.Services.User;
using PathwayGames.ViewModels;
using PathwayGames.Views;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PathwayGames
{
    public partial class App : Application
    {
        public static string DatabaseFilePath { get; private set; }
        public static int UserId;
        public static User SelectedUser;

        public static UnityContainer Container { get; private set; }
        public readonly static INavigationService NavigationService = new NavigationService();
        public App()
        {
            InitializeComponent();

            // Init DB
            InitializeDatabase();

            // Init DI
            InitializeDI();
        }

        private async Task InitializeNavigation()
        {
            MainPage = new MasterDetailView();

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

        private void InitializeDatabase()
        {
            DatabaseFilePath = DependencyService.Get<IFileAccessHelper>().GetDBPathAndCreateIfNotExists("pw.db");
        }

        private void InitializeDI()
        {
            Container = new UnityContainer();

            // Data repositories
            Container.RegisterType<IRepository<User>, Repository<User>>(new InjectionConstructor(DatabaseFilePath));

            // Infrastructure
            Container.RegisterInstance(NavigationService, new ContainerControlledLifetimeManager());
            Container.RegisterType<ISoundService, SoundService>();
            Container.RegisterType<IDialogService, DialogService>();

            // Services
            Container.RegisterType<ISlidesService, SlidesService>();
            Container.RegisterType<ISensorsService, SensorsService>();
            Container.RegisterType<IEngangementService, EngangementService>();
            Container.RegisterType<IExcelService, ExcelService>();
            Container.RegisterType<IUserService, UserService>();

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
            // Set selected user
            SelectedUser = await ServiceLocator.Current.GetInstance<IUserService>().GetSelectedUser();
            
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
