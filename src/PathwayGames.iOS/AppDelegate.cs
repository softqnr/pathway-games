﻿using Foundation;
using System;
using System.Threading.Tasks;
using UIKit;

namespace PathwayGames.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            AiForms.Renderers.iOS.SettingsViewInit.Init(); 
            global::Xamarin.Forms.FormsMaterial.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            LoadApplication(new App());

            // Catch unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += async (sender, e) => await UnhandledExceptionHandler(sender, e);
            TaskScheduler.UnobservedTaskException += async (sender, e) => await UnhandledExceptionHandler(sender, e);

            return base.FinishedLaunching(app, options);
        }

        private async Task UnhandledExceptionHandler(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("UNHANDLED EXCEPTION OCCURED");
            await Task.FromResult(true);
        }
    }
}
