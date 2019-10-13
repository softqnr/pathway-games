using PathwayGames.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class MasterViewModel : ViewModelBase
    {
        private User _user;

        public User User {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Version
        {
            get => $" v.{App.ApplicationVersion}";
        }

        public ICommand NavigationItemSelectedCommand
        {
            get
            {
                return new Command(async (sender) =>
                {
                    await NavigateTo(sender);
                });
            }
        }

        public ObservableCollection<NavMenuItem> MenuItems { get; set; }

        public MasterViewModel()
        {
            User = App.SelectedUser;

            MessagingCenter.Subscribe<User>(this, "Selected", (user) => {
                User = user;
            });

            MenuItems = new ObservableCollection<NavMenuItem>(new[]
            {
                    new NavMenuItem { Id = 0, Title = "Games", IconSource = "icon_tests.png", TargetType=typeof(GameSelectionViewModel) },
                    new NavMenuItem { Id = 1, Title = "Session data", IconSource = "icon_data.png", TargetType=typeof(SessionDataViewModel) },
                    new NavMenuItem { Id = 2, Title = "Users", IconSource = "icon_users.png", TargetType=typeof(UsersViewModel) },
                    new NavMenuItem { Id = 3, Title = "Game settings", IconSource = "icon_settings.png", TargetType=typeof(SettingsViewModel) },
            });
        }

        public async Task NavigateTo(object sender)
        {
            var item = sender as NavMenuItem;
            if (item == null)
                return;

            Title = item.Title;
            await NavigationService.NavigateToAsync(item.TargetType);
        }
    }
    public class NavMenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }
}
