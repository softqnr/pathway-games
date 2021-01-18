using PathwayGames.Models;
using PathwayGames.Services.User;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private ObservableCollection<User> _users;

        public string SelectedUserType { get; set; } = "All";

        public ObservableCollection<User> Users
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

        public ICommand DeleteUserCommand
        {
            get
            {
                return new Command(async (u) =>
                {
                    await OnUserDeletedCommand((User)u);
                });
            }
        }

        private async Task OnSearch(object searchText)
        {
            if (searchText != null)
            {
                // Do search
                Users = new ObservableCollection<User>(await _userService.GetByNameAndUserType((string)searchText, SelectedUserType));
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

                    DialogService.ShowToast(string.Format(Resources["TitleSelectedUserChangedTo"], user.UserName));
                    // Notify listeners
                    MessagingCenter.Send(user, "Selected");

                    // Navigate back
                    await NavigationService.NavigateBackAsync();
                } else {
                    DialogService.ShowToast(string.Format(Resources["TitleUserAllreadySelected"], App.SelectedUser.UserName));
                }
            }
        }

        public async Task OnAddUser()
        {
            await NavigationService.NavigateToPopupAsync<UserFormViewModel>(true);
        }

        private async Task OnUserDeletedCommand(Models.User selectedUser)
        {
            if (App.SelectedUser.IsAdmin)
            {
                if (App.SelectedUser == selectedUser) {
                    DialogService.ShowToast(Resources["TitleYouCannotDeleteYourself"]);
                } else {
                    bool confirmed = await DialogService.ShowConfirmAsync(Resources["TitleCannotUndoThisAction"],
                       Resources["PromptDeleteUser"], Resources["Ok"], Resources["Cancel"]);
                    if (confirmed)
                    {
                        await _userService.DeleteUser(selectedUser);
                        Users.Remove(selectedUser);
                    }
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
        }

        public override async Task InitializeAsync(object navigationData)
        {
            Users = new ObservableCollection<User>(await _userService.GetAll());
        }
    }
}
