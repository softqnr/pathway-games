using PathwayGames.Data;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private IUserService _userService;

        public User SelectedUser { get; set; }

        public string SelectedUserType { get; set; } = "All";

        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public ICommand SelectionChangedCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OnSelectionChanged();
                });
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new Command(() =>
                {
                    //await OnSelectionChanged();
                    DialogService.ShowToast("Add pressed");
                });
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new Command(async (s) =>
                {
                    await OnSearch(s);
                });
            }
        }

        private async Task OnSearch(object searchText)
        {
            if (searchText != null)
            {
                Users.Clear();
                // Do search
                foreach (var item in await _userService.GetByNameAndUserType((string)searchText, SelectedUserType)) Users.Add(item);
            }
            await Task.FromResult(true);
        }

        private async Task OnSelectionChanged()
        {
            if (SelectedUser.Id != App.SelectedUser.Id) {
                Models.User user = await _userService.SetSelectedUser(SelectedUser);
                App.SelectedUser = user;
                SelectedUser.IsSelected = true;
                // Notify listeners
                MessagingCenter.Send(user, "Selected");
            }
        }

        public UsersViewModel(IUserService userRepository)
        {
            _userService = userRepository;
        }
    }
}
