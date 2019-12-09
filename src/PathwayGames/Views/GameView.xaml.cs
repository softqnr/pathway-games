using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameView : ViewBase
    {
        public GameView()
        {
            InitializeComponent();
            SetLayout(DeviceOrientantion);
            OnOrientationChanged += DeviceOrientantionChanged;
        }

        private void DeviceOrientantionChanged(object sender, PageOrientationEventArgs e)
        {
            SetLayout(e.Orientation);
        }

        private void SetLayout(DisplayOrientation orientation)
        {
            switch (orientation)
            {
                case DisplayOrientation.Portrait:
                    BuzzerButton.VerticalOptions = LayoutOptions.End;
                    BuzzerButton.Margin = new Thickness(0);
                    GameSection.Orientation = StackOrientation.Vertical;
                    break;
                case DisplayOrientation.Landscape:
                    BuzzerButton.VerticalOptions = LayoutOptions.Center;
                    BuzzerButton.Margin = new Thickness(0,0,12,0);
                    GameSection.Orientation = StackOrientation.Horizontal;
                    break;
            }
        }

        protected override void OnDisappearing()
        {
            OnOrientationChanged -= DeviceOrientantionChanged;
            base.OnDisappearing();
        }
    }
}