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