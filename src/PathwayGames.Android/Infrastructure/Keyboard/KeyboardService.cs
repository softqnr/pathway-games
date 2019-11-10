using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using PathwayGames.Droid.Infrastructure.Keyboard;
using PathwayGames.Infrastructure.Keyboard;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeyboardService))]
namespace PathwayGames.Droid.Infrastructure.Keyboard
{
    public class KeyboardService : IKeyboardService
    {
        public void HideKeyboard()
        {
            var context = Android.App.Application.Context;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && context is Activity)
            {
                var activity = context as Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}