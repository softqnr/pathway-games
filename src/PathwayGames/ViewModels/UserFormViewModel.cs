using PathwayGames.Infrastructure.Device;
using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using PathwayGames.Services.User;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class UserFormViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IDeviceHelper _deviceHelper;
        private string _userName;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserType { get; set; } = Models.Enums.UserType.Learner.ToString();

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
            if (!IsBusy)
            {
                if (UserName != "") {
                    IsBusy = true;
                    Enum.TryParse<UserType>(UserType, out var userType);
                    int ppi = _deviceHelper.MachineNameToPPI(DeviceInfo.Model);
                    EyeGazeCompensation compensantion = _deviceHelper.MachineNameToEyeGazeCompensation(DeviceInfo.Model);
                    await _userService.CreateUser(UserName, userType, ppi, compensantion.WidthCompensation, compensantion.HeightCompensation);
                    DialogService.ShowToast(Resources["TitleUserAdded"]);
                    await NavigationService.PopAsync(true);
                } else {
                    DialogService.ShowToast(Resources["TitleUserNameCannotBeEmpty"]);
                }
            }
        }

        public UserFormViewModel(IUserService userService)
        {
            _userService = userService;
            _deviceHelper = DependencyService.Get<IDeviceHelper>();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await Task.FromResult(true);
        }
    }
}
