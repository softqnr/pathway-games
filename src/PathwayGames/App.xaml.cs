using CommonServiceLocator;
using Newtonsoft.Json;
using PathwayGames.Controls;
using PathwayGames.Data;
using PathwayGames.Infrastructure.Dialog;
using PathwayGames.Infrastructure.File;
using PathwayGames.Infrastructure.Json;
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Xamarin.Essentials;
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
        public static string LocalStorageDirectory = FileSystem.AppDataDirectory;
        public static string ApplicationVersion = $"{VersionTracking.CurrentVersion}.{VersionTracking.CurrentBuild}";

        public static IUnityContainer Container { get; private set; }
        public readonly static INavigationService NavigationService = new NavigationService();
        public App()
        {
            InitializeComponent();

            // Enable flags
            Device.SetFlags(new[] {
                "StateTriggers_Experimental",
                "Shapes_Experimental",
                "Brush_Experimental"
            });

            // Init version tracking
            VersionTracking.Track();

            // Init DB
            InitializeDatabase();

            // Init DI
            InitializeDependencies();

            // Init JSON serialization
            InitializeJson();
        }

        private async Task InitializeNavigation()
        {
            MainPage = new MasterDetailView();
            ((MasterDetailView)MainPage).MasterBehavior = MasterBehavior.Popover;

            NavigationService.Configure(typeof(MasterViewModel), typeof(MasterView));
            NavigationService.Configure(typeof(GameSelectionViewModel), typeof(GameSelectionView));
            NavigationService.Configure(typeof(GameViewModel), typeof(GameView));
            NavigationService.Configure(typeof(SettingsViewModel), typeof(SettingsView));
            NavigationService.Configure(typeof(GameResultsViewModel), typeof(GameResultsView));
            NavigationService.Configure(typeof(SessionDataViewModel), typeof(SessionDataView));
            NavigationService.Configure(typeof(SensorsViewModel), typeof(SensorsView));
            NavigationService.Configure(typeof(UsersViewModel), typeof(UsersView));
            NavigationService.Configure(typeof(UserFormViewModel), typeof(UserFormView));

            await NavigationService.InitializeAsync();
        }

        private void InitializeJson()
        {
            // Ignore some properties that we do not have access to and add converters
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = ShouldSerializeContractResolver.Instance,
                //Converters = new List<JsonConverter> { new UnixTimeMillisecondsConverter() },
                Formatting = Formatting.Indented,
            };
        }

        private void InitializeDatabase()
        {
            DatabaseFilePath = DependencyService.Get<IFileAccessHelper>().GetDBPathAndCreateIfNotExists("pw.db");
        }

        private void InitializeDependencies()
        {
            Container = new UnityContainer().AddExtension(new ForceActivation()); // IOS bug fix see https://github.com/unitycontainer/container/issues/150

            // Data repositories
            Container.RegisterType<IRepository<User>, Repository<User>>(new InjectionConstructor(DatabaseFilePath));
            Container.RegisterType<IRepository<UserGameSettings>, Repository<UserGameSettings>>(new InjectionConstructor(DatabaseFilePath));
            Container.RegisterType<IRepository<UserGameSession>, Repository<UserGameSession>>(new InjectionConstructor(DatabaseFilePath));
            Container.RegisterType<IRepository<SeekGridOption>, Repository<SeekGridOption>>(new InjectionConstructor(DatabaseFilePath));

            // Infrastructure
            Container.RegisterInstance(NavigationService, new ContainerControlledLifetimeManager());
            Container.RegisterType<ISoundService, SoundService>();
            Container.RegisterType<IDialogService, DialogService>();

            // Services
            Container.RegisterType<ISlidesService, SlidesService>();
            Container.RegisterType<ISensorLogWriterService, SensorLogWriterService>();
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
            Container.RegisterType<UserFormViewModel>();

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
