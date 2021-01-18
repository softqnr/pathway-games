﻿using System.Globalization;

namespace PathwayGames.Localization
{
    public class CultureChangedMessage
    {
        public CultureInfo NewCultureInfo { get; private set; }

        public CultureChangedMessage(string lngName)
            : this(new CultureInfo(lngName))
        { }

        public CultureChangedMessage(CultureInfo newCultureInfo)
        {
            NewCultureInfo = newCultureInfo;
        }
    }
}