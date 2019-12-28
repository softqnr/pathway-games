using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersView : ContentPage
    {
        public UsersView()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            if (!App.SelectedUser.IsAdmin)
            {
                ToolbarItems.Remove(AddUserButton);
            }
        }

    }
}