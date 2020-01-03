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

        private void FaceSensorView_TrackingStarted(object sender, System.EventArgs e)
        {
            EyeGazeIcon.FadeTo(0, 500);
            EyeGazeIcon.TextColor = Color.Black;
            EyeGazeIcon.FadeTo(100, 500);
        }

        private void FaceSensorView_TrackingStopped(object sender, System.EventArgs e)
        {
            EyeGazeIcon.FadeTo(0, 500);
            EyeGazeIcon.TextColor = Color.Red;
            EyeGazeIcon.FadeTo(100, 500);
        }
    }
}