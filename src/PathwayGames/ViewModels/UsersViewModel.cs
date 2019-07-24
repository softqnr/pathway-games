using PathwayGames.Data;
using PathwayGames.Forms;
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
        private IList<User> _users;
        private bool _currentUserIsAdmin;

        public bool CurrentUserIsAdmin
        {
            get => _currentUserIsAdmin;
            set => SetProperty(ref _currentUserIsAdmin, value);
        }

        public string SelectedUserType { get; set; } = "All";

        //public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public IList<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ICommand UserSelectedCommand
        {
            get
            {
                return new Command(async (u) =>
                {
                    await OnUserSelectedCommand((User)u);
                });
            }
        }

        public ICommand GotoUserSessionsCommand
        {
            get
            {
                return new Command(async (u) =>
                {
                    await OnGotoUserSessionsCommand((User)u);
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
                //Users.Clear();
                //while (Users.Count > 0)
                //    Users.RemoveAt(0);
                // Do search
                //foreach (var item in await _userService.GetByNameAndUserType(searchText, SelectedUserType)) Users.Add(item);
                Users = await _userService.GetByNameAndUserType((string)searchText, SelectedUserType);
            }
        }

        private async Task OnUserSelectedCommand(Models.User selectedUser)
        {
            if (selectedUser != null)
            {
                if ( App.SelectedUser.Id != selectedUser.Id)
                {
                    Models.User user = await _userService.SetSelectedUser(selectedUser);
                    App.SelectedUser = user;
                    CurrentUserIsAdmin = user.IsAdmin;

                    DialogService.ShowToast($"Selected user changed to {user.UserName}");
                    // Notify listeners
                    MessagingCenter.Send(user, "Selected");
                }
            }
        }

        private async Task OnGotoUserSessionsCommand(User user)
        {
            await NavigationService.NavigateToAsync<SessionDataViewModel>(user);
        }

        public UsersViewModel(IUserService userService)
        {
            _userService = userService;
            CurrentUserIsAdmin = App.SelectedUser.IsAdmin;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await Task.FromResult(true);
        }
    }
}
