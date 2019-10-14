using PathwayGames.Infrastructure.Share;
using PathwayGames.Models;
using PathwayGames.Services.User;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SessionDataViewModel : ViewModelBase
    {
        private IUserService _userService;
        private string _title;
        public ObservableCollection<UserGameSession> GameSessions { get; } = new ObservableCollection<UserGameSession>();

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ICommand GameDataCommand
        {
            get
            {
                return new Command((s) =>
                {
                    ShareGameData((string)s);
                });
            }
        }

        public ICommand SensorDataCommand
        {
            get
            {
                return new Command((s) =>
                {
                    ShareSensorData((string)s);
                });
            }
        }

        private void ShareGameData(string gameDataFile)
        {
            string filePath = Path.Combine(App.LocalStorageDirectory, gameDataFile);
            DependencyService.Get<IShare>().ShareFile("Share game data", "Share game data", filePath);
        }

        private void ShareSensorData(string sensorDataFile)
        {
            string filePath = Path.Combine(App.LocalStorageDirectory, sensorDataFile);
            DependencyService.Get<IShare>().ShareFile("Share sensor data", "Share sensor data", filePath);
        }

        public SessionDataViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData == null)
            {
                Title = "Session data - " + App.SelectedUser.UserName;
                // Show current selected users game sessions
                foreach (var item in await _userService.GetUserGameSessions(App.SelectedUser.Id)) GameSessions.Add(item);
            } else {
                User user = navigationData as User;
                Title = "Session data - " + user.UserName;
                // Show users game sessions
                foreach (var item in await _userService.GetUserGameSessions(user.Id)) GameSessions.Add(item);
            }
        }
    }
}
