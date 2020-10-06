using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using System;
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

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            AiForms.Renderers.Droid.SettingsViewInit.Init();

            // Catch unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += async (sender, e) => await UnhandledExceptionHandler(sender, e);
            TaskScheduler.UnobservedTaskException += async (sender, e) => await UnhandledExceptionHandler(sender, e);
            AndroidEnvironment.UnhandledExceptionRaiser += async (sender, e) => await UnhandledExceptionHandler(sender, e);

            // Init FFImageLoading
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            LoadApplication(new App());
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                System.Diagnostics.Debug.WriteLine("Android back button: There are some pages in the PopupStack");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Android back button: There are not any pages in the PopupStack");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async Task UnhandledExceptionHandler(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("UNHANDLED EXCEPTION OCCURED");
            await Task.FromResult(true);
        }
    }
}