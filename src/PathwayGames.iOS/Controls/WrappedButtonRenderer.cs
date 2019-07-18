using PathwayGames.Controls;
using PathwayGames.iOS.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(WrappedButtonRenderer))]
namespace PathwayGames.iOS.Controls
{
    public class WrappedButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.TitleEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
                Control.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
                Control.TitleLabel.TextAlignment = UITextAlignment.Center;
            }
        }
    }
}