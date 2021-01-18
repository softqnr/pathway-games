using PathwayGames.Helpers;
using PathwayGames.Localization;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames.Views
{
    public partial class GameSelectionView : ViewBase
    {
        public GameSelectionView()
        {
            InitializeComponent();
            SetLayout(DeviceOrientantion);
            OnOrientationChanged += DeviceOrientantionChanged;
        }

        private void DeviceOrientantionChanged(object sender, PageOrientationEventArgs e)
        {
            SetLayout(e.Orientation);
        }

        private void SetLayout(DisplayOrientation orientation)
        {
            switch (orientation)
            {
                case DisplayOrientation.Portrait:
                    OptionsStack.Orientation = StackOrientation.Vertical;
                    foreach (var children in OptionsStack.Children)
                    {
                        children.HorizontalOptions = LayoutOptions.FillAndExpand;
                        children.VerticalOptions = LayoutOptions.Center;
                    }
                    break;
                case DisplayOrientation.Landscape:
                    OptionsStack.Orientation = StackOrientation.Horizontal;
                    foreach (var children in OptionsStack.Children)
                    {
                        children.HorizontalOptions = LayoutOptions.StartAndExpand;
                        children.VerticalOptions = LayoutOptions.FillAndExpand;
                    }
                    break;
            }
        }

        protected override void OnDisappearing()
        {
            OnOrientationChanged -= DeviceOrientantionChanged;
            base.OnDisappearing();
        }
    }
}
