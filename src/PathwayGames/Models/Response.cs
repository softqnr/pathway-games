using System;
using System.Collections.Generic;
using PathwayGames.Models.Enums;

namespace PathwayGames.Models
{
    public class Response
    {
        public int SlideNumber { get; set; }
        public string SlideName { get; set; }
        public DateTime SlideDisplayed { get; set; }
        public DateTime SlideHidden { get; set; }
        public IList<ButtonPress> ButtonPresses { get; set; }
        public ResponseResult ResponseResult { get; set; }
        public DateTime ResponseTime { get; set; }
        public int Points { get; set; }
    }
}
