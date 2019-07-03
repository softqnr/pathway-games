using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PathwayGames.Controls
{
    public class EyeGazePreview : View
    {
        public delegate void LookAtPointHandler(object sender, LookAtPointEventArgs e);
        public event LookAtPointHandler OnLookAtPoint;
    }
}
