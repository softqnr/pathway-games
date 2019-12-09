using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private IList<User> _users;

        public string SelectedUserType { get; set; } = "All";

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
                return new Command(async () =>
                {
                    await OnAddUser();
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

                    DialogService.ShowToast($"Selected user changed to {user.UserName}");
                    // Notify listeners
                    MessagingCenter.Send(user, "Selected");
                }
            }
        }

        public async Task OnAddUser()
        {
            await NavigationService.NavigateToPopupAsync<UserFormViewModel>(true);
        }

        private async Task OnGotoUserSessionsCommand(User user)
        {
            await NavigationService.NavigateToAsync<SessionDataViewModel>(user);
        }

        public UsersViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await Task.FromResult(true);
        }
    }
}
