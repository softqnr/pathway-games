using System;
using Xamarin.Forms;

namespace PathwayGames.Controls
{
    public class EyeGazePreview : View
    {
        public event EventHandler<EyeGazeChangedEventArgs> OnEyeGazeChanged;
    }
}
