using PathwayGames.ViewModels;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewBase : ContentPage
	{
        public event EventHandler<PageOrientationEventArgs> OnOrientationChanged = (e, a) => { };
        public DisplayOrientation DeviceOrientantion { get; private set; }

        public ViewBase ()
		{
			InitializeComponent ();
            Init();
        }

        private void Init()
        {
            DeviceOrientantion = DeviceDisplay.MainDisplayInfo.Orientation;
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            // Has the device been rotated ?
            if (!Equals(DeviceOrientantion, e.DisplayInfo.Orientation))
            {
                DeviceOrientationChange(e.DisplayInfo.Orientation);
            }
        }
        
        private void DeviceOrientationChange(DisplayOrientation orientation)
        {
            DeviceOrientantion = orientation;
            OnOrientationChanged.Invoke(this, new PageOrientationEventArgs(orientation));
        }

        protected override bool OnBackButtonPressed()
        {
            var bindingContext = BindingContext as ViewModelBase;

            if (bindingContext != null)
                bindingContext.OnBackButtonPressed();

            return base.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var bindingContext = BindingContext as ViewModelBase;

            if (bindingContext != null)
                bindingContext.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            var bindingContext = BindingContext as ViewModelBase;

            if (bindingContext != null)
                bindingContext.OnDisappearing();
        }
    }

    public class PageOrientationEventArgs : EventArgs
    {
        public PageOrientationEventArgs(DisplayOrientation orientation)
        {
            Orientation = orientation;
        }

        public DisplayOrientation Orientation { get; }
    }
}