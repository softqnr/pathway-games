using System;
using System.Text;

using Foundation;
using UIKit;
using ARKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CoreGraphics;
using SceneKit;
using CoreFoundation;

[assembly: ExportRenderer(typeof(PathwayGames.Views.GameView), typeof(PathwayGames.iOS.Sensors.EyeDetectionRenderer))]
namespace PathwayGames.iOS.Sensors
{
    public class EyeDetectionRenderer : PageRenderer
    {
        private ARSCNView SceneView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Enable AR
            SceneView = new ARSCNView
            {
                // 263, 668, 96, 128
                Frame = new CGRect(263, 558, 96, 128),
                ContentMode = UIViewContentMode.ScaleToFill,
                UserInteractionEnabled = true,
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutomaticallyUpdatesLighting = true,
                // UITapGestureRecognizer =  new   UITapGestureRecognizer(),
                // Frame = new CGRect(View.Frame.X, View.Frame.Y, View.Frame.Width, View.Frame.Height),
                // ShowsStatistics = true, // Show stats

            };
            View.AddSubview(SceneView);
            SceneView.Delegate = new EyeDetectionDelegate(SceneView);
            //sceneView.Session.Delegate = new SessionDelegate();
            
            // Send to back
            //View.SendSubviewToBack(SceneView);
            //sceneView.Scene.Background.Contents = UIColor.White;
        }     

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Create a session configuration
            if (!ARFaceTrackingConfiguration.IsSupported)
            {
                ShowUnsupportedDeviceError();
                return;
            }
            ARFaceTrackingConfiguration configuration = new ARFaceTrackingConfiguration
            {
                LightEstimationEnabled = true
            };

            // Run the view's session options
            SceneView.Session.Run(configuration, 
                ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
    }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Pause the view's session
            SceneView.Session.Pause();
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
        }

        private void ShowUnsupportedDeviceError()
        {
            var alertController = UIAlertController.Create("ARKit is not available on this device.",
                "This app requires world tracking, which is available only on iOS devices with the A9 processor or later.",
                UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.PresentModalViewController(alertController, true);
        }
    }
}