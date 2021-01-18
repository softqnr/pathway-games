using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Newtonsoft.Json;
using PathwayGames.Infrastructure.Data;
using PathwayGames.Infrastructure.Dialog;
using PathwayGames.Infrastructure.File;
using PathwayGames.Infrastructure.Json;
using PathwayGames.Infrastructure.Navigation;
using PathwayGames.Infrastructure.Sound;
using PathwayGames.Models;
using PathwayGames.Services.Excel;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.Services.User;
using PathwayGames.ViewModels;
using PathwayGames.Views;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames
{
    public partial class App : Application
    {
        public static string DatabaseFilePath { get; private set; }
        public static int UserId;
        public static User SelectedUser;
        public static string LocalStorageDirectory = FileSystem.AppDataDirectory;
        public static string ApplicationVersion = $"{VersionTracking.CurrentVersion}.{VersionTracking.CurrentBuild}";
        public static Language CurrentLanguage
        {
            get => new Language(SelectedUser.Language);
            set => SelectedUser.Language = value.ShortName;
        }

        public static IContainer Container { get; private set; }
        public readonly static INavigationService NavigationService = new NavigationService();
        public App()
        {
            InitializeComponent();

            //Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("sr");

            // Enable flags for experimental features
            Device.SetFlags(new[] {
                "StateTriggers_Experimental",
                "Shapes_Experimental",
                "Brush_Experimental",
                "RadioButton_Experimental"
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
            NavigationService.Configure(typeof(LanguagesViewModel), typeof(LanguagesView));

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
            var builder = new ContainerBuilder();
            // Data repositories
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>)).WithParameter("databaseFile", DatabaseFilePath); ;
            //builder.RegisterType<IRepository<User>>()
            //    .As<Repository<User>>().WithParameter("databaseFile", DatabaseFilePath);
            //builder.RegisterType<IRepository<UserGameSettings>>()
            //    .As<Repository<UserGameSettings>>().WithParameter("databaseFile", DatabaseFilePath);
            //builder.RegisterType<IRepository<UserGameSession>>()
            //    .As<Repository<UserGameSession>>().WithParameter("databaseFile", DatabaseFilePath);
            //builder.RegisterType<IRepository<SeekGridOption>>()
            //    .As<Repository<SeekGridOption>>().WithParameter("databaseFile", DatabaseFilePath);

            // Infrastructure
            builder.RegisterInstance(NavigationService).SingleInstance();
            builder.RegisterType<SoundService>().As<ISoundService>();
            builder.RegisterType<DialogService>().As<IDialogService>();

            // Services
            builder.RegisterType<SlidesService>().As<ISlidesService>();
            builder.RegisterType<SensorLogWriterService>().As<ISensorLogWriterService>();
            builder.RegisterType<ExcelService>().As<IExcelService>();
            builder.RegisterType<UserService>().As<IUserService>();
            //builder.RegisterType<EngangementService>().As<IEngangementService>();

            // View models
            builder.RegisterType<MasterViewModel>();
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<GameSelectionViewModel>();
            builder.RegisterType<GameViewModel>();
            builder.RegisterType<SettingsViewModel>();
            builder.RegisterType<GameResultsViewModel>();
            builder.RegisterType<SessionDataViewModel>();
            builder.RegisterType<SensorsViewModel>();
            builder.RegisterType<UsersViewModel>();
            builder.RegisterType<UserFormViewModel>();
            builder.RegisterType<LanguagesViewModel>();

            // Set as service locator provider
            Container = builder.Build();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(Container));
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
