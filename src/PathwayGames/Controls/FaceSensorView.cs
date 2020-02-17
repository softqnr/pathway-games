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
            BindableProperty.Create(nameof(RecordingEnabled), typeof(bool), typeof(FaceSensorView), false);

        public static readonly BindableProperty EyeGazeVisualizationEnabledProperty =
            BindableProperty.Create(nameof(EyeGazeVisualizationEnabled), typeof(bool), typeof(FaceSensorView), true);

        public static readonly BindableProperty CameraPreviewEnabledProperty =
            BindableProperty.Create(nameof(CameraPreviewEnabled), typeof(bool), typeof(FaceSensorView), true);

        public static readonly BindableProperty ScreenPPIProperty =
            BindableProperty.Create(nameof(ScreenPPI), typeof(int), typeof(FaceSensorView), 0);

        public static readonly BindableProperty WidthCompensationProperty =
            BindableProperty.Create(nameof(WidthCompensation), typeof(float), typeof(FaceSensorView), 0f);

        public static readonly BindableProperty HeightCompensationProperty =
            BindableProperty.Create(nameof(HeightCompensation), typeof(float), typeof(FaceSensorView), 0f);

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

        public bool EyeGazeVisualizationEnabled
        {
            get { return (bool)GetValue(EyeGazeVisualizationEnabledProperty); }
            set { SetValue(EyeGazeVisualizationEnabledProperty, value); }
        }

        public bool CameraPreviewEnabled
        {
            get { return (bool)GetValue(CameraPreviewEnabledProperty); }
            set { SetValue(CameraPreviewEnabledProperty, value); }
        }

        public int ScreenPPI 
        { 
            get => (int) GetValue(ScreenPPIProperty);
            set => SetValue(ScreenPPIProperty, value); 
        }

        public float WidthCompensation { 
            get => (float)GetValue(WidthCompensationProperty); 
            set => SetValue(WidthCompensationProperty, value); 
        }

        public float HeightCompensation {
            get => (float)GetValue(HeightCompensationProperty);
            set => SetValue(HeightCompensationProperty, value);
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
