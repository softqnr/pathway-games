using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.Controls
{
    public class HideableToolbarItem : ToolbarItem
    {
        private string _oldText = "";
        private ICommand oldCommand = null;

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static readonly BindableProperty IsVisibleProperty =
             BindableProperty.Create(nameof(IsVisible),
               typeof(bool),
               typeof(HideableToolbarItem),
               true,
               propertyChanged: OnIsVisibleChanged);

        private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = bindable as HideableToolbarItem;

            var newValueBool = (bool)newValue;
            var oldValueBool = (bool)oldValue;

            if (!newValueBool && oldValueBool)
            {
                item._oldText = item.Text;
                item.oldCommand = item.Command;
                item.Text = "";
                item.Command = null;
            }

            if (newValueBool && !oldValueBool)
            {
                item.Text = item._oldText;
                item.Command = item.oldCommand;
            }
        }
    }
}
