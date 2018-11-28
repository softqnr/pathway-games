﻿using System;
using Xamarin.Forms;

namespace PathwayGames.Models
{
    public class ButtonPress
    {
        public int? SlideIndex { get; set; }
        public DateTime Time { get; set; }
        public Point Coordinates { get; set; }

        public override string ToString()
        {
            return $"{Time:dd/MM/yyyy HH:mm:ss.fff} - ({Coordinates.X}, {Coordinates.Y} )"; 
        }
    }
}
