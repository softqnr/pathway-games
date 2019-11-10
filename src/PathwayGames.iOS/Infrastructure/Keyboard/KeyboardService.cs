using PathwayGames.Infrastructure.Keyboard;
using PathwayGames.iOS.Infrastructure.Keyboard;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(KeyboardService))]
namespace PathwayGames.iOS.Infrastructure.Keyboard
{
    public class KeyboardService : IKeyboardService
    {
        public void HideKeyboard()
        {
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
        }
    }
}