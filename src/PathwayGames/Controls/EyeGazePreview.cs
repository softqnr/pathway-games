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

        public ICommand EyeGazeChangedCommand
        {
            get { return (ICommand)GetValue(EyeGazeChangedCommandProperty); }
            set { SetValue(EyeGazeChangedCommandProperty, value); }
        }

        void ISensor.OnEyeGazeChanged(EyeGazeChangedEventArgs e)
        {
            EyeGazeChanged?.Invoke(this, e);
            EyeGazeChangedCommand?.Execute(e);
        }
    }
}
