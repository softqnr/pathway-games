using System;
using Xamarin.Forms;

namespace PathwayGames.Behaviors
{
    public class NumericEntryBehavior : Behavior<Entry>
    {
        protected Action<Entry, string> AdditionalCheck;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.TextChanged += TextChanged_Handler;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
        }

        protected virtual void TextChanged_Handler(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                ((Entry)sender).Text = 0.ToString();
                return;
            }

            double _;
            if (!double.TryParse(e.NewTextValue, out _))
                ((Entry)sender).Text = e.OldTextValue;
            else
                AdditionalCheck?.Invoke(((Entry)sender), e.OldTextValue);
        }
    }
}
