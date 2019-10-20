using PathwayGames.Models;
using PathwayGames.Services.User;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
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
                return new Command(async (s) =>
                {
                    await ShareGameData((string)s);
                });
            }
        }

        public ICommand SensorDataCommand
        {
            get
            {
                return new Command(async (s) =>
                {
                    await ShareSensorData((string)s);
                });
            }
        }

        private async Task ShareGameData(string gameDataFile)
        {
            var message = new EmailMessage
            {
                Subject = "Pathway+ Games - Test results",
                Body = "",
            };

            message.Attachments.Add(new EmailAttachment(Path.Combine(App.LocalStorageDirectory, gameDataFile)));

            await Email.ComposeAsync(message);
        }

        private async Task ShareSensorData(string sensorDataFile)
        {
            var message = new EmailMessage
            {
                Subject = "Pathway+ Games - Sensor data",
                Body = "",
            };

            message.Attachments.Add(new EmailAttachment(Path.Combine(App.LocalStorageDirectory, sensorDataFile)));

            await Email.ComposeAsync(message);
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
