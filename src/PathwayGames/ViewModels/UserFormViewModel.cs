using PathwayGames.Models;
using System;
using PathwayGames.Models.Enums;
using PathwayGames.Services.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class UserFormViewModel : ViewModelBase
    {
        private IUserService _userService;
        private IList<User> _users;
        private string _userName;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserType { get; set; } = "Learner";

        public ICommand SaveUserCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OnSaveUser();
                });
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await NavigationService.PopAsync(true);
                });
            }
        }

        public async Task OnSaveUser()
        {
            if (UserName != "")
            {
                Enum.TryParse<UserType>(UserType, out var userType);
                await _userService.CreateUser(UserName, userType);
                DialogService.ShowToast("User added");
                await NavigationService.PopAsync(true);
            }
            else
            {
                DialogService.ShowToast("User name cannot be empty!");
            }
        }

        public UserFormViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await Task.FromResult(true);
        }
    }
}
