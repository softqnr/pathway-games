using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorsView : ViewBase
    {
        public SensorsView()
        {
            InitializeComponent();
            DeviceDisplay.KeepScreenOn = true;
            OnOrientationChanged += DeviceOrientantionChanged;
        }

        private void DeviceOrientantionChanged(object sender, PageOrientationEventArgs e)
        {
            SetLayout(e.Orientation);
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

        private void SetLayout(DisplayOrientation orientation)
        {
            switch (orientation)
            {
                case DisplayOrientation.Portrait:
                    SensorScreen.Orientation = StackOrientation.Vertical;
                    break;
                case DisplayOrientation.Landscape:
                    SensorScreen.Orientation = StackOrientation.Horizontal;
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