using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace PathwayGames.Droid
{
    [Activity(Label = "Pathway+ Games", Icon = "@mipmap/ic_launcher_circle", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            CachedImageRenderer.Init(enableFastRenderer: true);
            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);

            // Catch unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += async (sender, e) => await UnhandledExceptionHandler(sender, e);
            TaskScheduler.UnobservedTaskException += async (sender, e) => await UnhandledExceptionHandler(sender, e);
            AndroidEnvironment.UnhandledExceptionRaiser += async (sender, e) => await UnhandledExceptionHandler(sender, e);

            LoadApplication(new App());
        }

        private async Task UnhandledExceptionHandler(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("UNHANDLED EXCEPTION OCCURED");
            await Task.FromResult(true);
        }
    }
}