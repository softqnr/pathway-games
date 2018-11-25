using PathwayGames.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterView : ContentPage
    {
        public MasterView()
        {
            InitializeComponent();
            // TODO: Change this 
            BindingContext = new MasterViewModel();
        }
    }
}