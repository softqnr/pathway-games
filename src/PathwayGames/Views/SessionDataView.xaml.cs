using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionDataView : ContentPage
    {
        public SessionDataView()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            if (!App.SelectedUser.IsAdmin)
            {
                //ToolbarItem toolbarItem = new ToolbarItem();
                //toolbarItem.SetBinding(ToolbarItem.CommandProperty, "ExportAllUserDataCommand");
                //toolbarItem.IconImageSource = new FontImageSource
                //{
                //    FontFamily = "Material",
                //    Glyph = Application.Current.Resources["IconDownload"].ToString()
                //};
                //ToolbarItems.Add(toolbarItem);
                ToolbarItems.Remove(ExportAllToolbarItem);
            }
        }

    }
}