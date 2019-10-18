using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.Controls
{
    public class EyeGazePreview : View, ISensor
    {
        public event EventHandler<EyeGazeChangedEventArgs> EyeGazeChanged;

        public static readonly BindableProperty EyeGazeChangedCommandProperty =
            BindableProperty.Create(nameof(EyeGazeChangedCommand), typeof(ICommand), typeof(EyeGazePreview), null);

        public static readonly BindableProperty RecordingEnabledProperty =
            BindableProperty.Create(nameof(RecordingEnabled), typeof(bool), typeof(EyeGazePreview), null);

        public static readonly BindableProperty ShowCrosshairProperty =
            BindableProperty.Create(nameof(ShowCrosshair), typeof(bool), typeof(EyeGazePreview), null);

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

        void ISensor.OnEyeGazeChanged(EyeGazeChangedEventArgs e)
        {
            if (RecordingEnabled)
            {
                EyeGazeChanged?.Invoke(this, e);
                EyeGazeChangedCommand?.Execute(e);
            }
        }
    }
}
