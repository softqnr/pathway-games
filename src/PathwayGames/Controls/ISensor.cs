using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Controls
{
    public interface ISensor
    {
        void OnEyeGazeChanged(EyeGazeChangedEventArgs e);
    }
}
