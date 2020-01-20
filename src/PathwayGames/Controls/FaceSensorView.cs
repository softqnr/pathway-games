using PathwayGames.Sensors;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.Controls
{
    public class FaceSensorView : View, IFaceSensor
    {
        public event EventHandler<FaceAnchorChangedEventArgs> EyeGazeChanged;
        public event EventHandler<EventArgs> TrackingStarted;
        public event EventHandler<EventArgs> TrackingStopped;
        
        public static readonly BindableProperty EyeGazeChangedCommandProperty =
            BindableProperty.Create(nameof(EyeGazeChangedCommand), typeof(ICommand), typeof(FaceSensorView), null);

        public static readonly BindableProperty RecordingEnabledProperty =
            BindableProperty.Create(nameof(RecordingEnabled), typeof(bool), typeof(FaceSensorView), null);

        public static readonly BindableProperty ShowCrosshairProperty =
            BindableProperty.Create(nameof(ShowCrosshair), typeof(bool), typeof(FaceSensorView), null);

        public static readonly BindableProperty ShowPreviewProperty =
            BindableProperty.Create(nameof(ShowPreview), typeof(bool), typeof(FaceSensorView), null);

        public ICommand EyeGazeChangedCommand
        {
            get { return (ICommand)GetValue(EyeGazeChangedCommandProperty); }
            set { SetValue(EyeGazeChangedCommandProperty, value); }
        }

        public bool RecordingEnabled
        {
            get { return (bool)GetValue(RecordingEnabledProperty); }
            set { SetValue(RecordingEnabledProperty, value); }
        }

        public bool ShowCrosshair
        {
            get { return (bool)GetValue(ShowCrosshairProperty); }
            set { SetValue(ShowCrosshairProperty, value); }
        }

        public bool ShowPreview
        {
            get { return (bool)GetValue(ShowPreviewProperty); }
            set { SetValue(ShowPreviewProperty, value); }
        }

        void ISensor<FaceAnchorChangedEventArgs>.OnReadingTaken(FaceAnchorChangedEventArgs e)
        {
            if (RecordingEnabled)
            {
                EyeGazeChanged?.Invoke(this, e);
                EyeGazeChangedCommand?.Execute(e);
            }
        }

        void ISensor<FaceAnchorChangedEventArgs>.OnTrackingStarted(EventArgs e)
        {
            TrackingStarted?.Invoke(this, e);
        }

        void ISensor<FaceAnchorChangedEventArgs>.OnTrackingStopped(EventArgs e)
        {
            TrackingStopped?.Invoke(this, e);
        }
    }
}
