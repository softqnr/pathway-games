using PathwayGames.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewBase : ContentPage
	{
		public ViewBase ()
		{
			InitializeComponent ();
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
}