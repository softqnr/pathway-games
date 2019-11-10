using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;

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
                    gridButtonStyles.Setters.Where(x => x.Property.PropertyName == "HorizontalOptions")
                        .SingleOrDefault().Value =LayoutOptions.FillAndExpand;
                    break;
                case DisplayOrientation.Landscape:
                    OptionsStack.Orientation = StackOrientation.Horizontal;
                    gridButtonStyles.Setters.Where(x => x.Property.PropertyName == "HorizontalOptions")
                        .SingleOrDefault().Value = LayoutOptions.Start;
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
