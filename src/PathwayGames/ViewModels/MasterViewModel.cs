using PathwayGames.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class MasterViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
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
            MenuItems = new ObservableCollection<NavMenuItem>(new[]
            {
                    new NavMenuItem { Id = 0, Title = "Tests", IconSource = "icon_tests.png", TargetType=typeof(GameSelectionViewModel) },
                    new NavMenuItem { Id = 1, Title = "Session data", IconSource = "icon_data.png", TargetType=typeof(SessionDataViewModel) },
                    new NavMenuItem { Id = 2, Title = "Users", IconSource = "icon_users.png", TargetType=typeof(UsersViewModel) },
                    new NavMenuItem { Id = 3, Title = "Settings", IconSource = "icon_settings.png", TargetType=typeof(SettingsViewModel) },
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
