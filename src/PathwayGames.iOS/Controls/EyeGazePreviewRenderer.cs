using ARKit;
using CoreGraphics;
using PathwayGames.Controls;
using PathwayGames.iOS.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EyeGazePreview), typeof(EyeGazePreviewRenderer))]
namespace PathwayGames.iOS.Controls
{
    public class EyeGazePreviewRenderer : ViewRenderer<EyeGazePreview, ARSCNView>
    {
        private ARSCNView SceneView;
     
        protected override void OnElementChanged(ElementChangedEventArgs<EyeGazePreview> e)
        {
            base.OnElementChanged(e);
            //e.NewElement.OnEyeGazeChanged.

            if (Control == null)
            {
                // Enable AR
                SceneView = new ARSCNView
                {
                    // 263, 668, 96, 128
                    Frame = new CGRect(263, 558, 96, 128),
                    ContentMode = UIViewContentMode.ScaleToFill,
                    UserInteractionEnabled = true,
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    AutomaticallyUpdatesLighting = true,
                    // UITapGestureRecognizer =  new UITapGestureRecognizer(),
                    // Frame = new CGRect(View.Frame.X, View.Frame.Y, View.Frame.Width, View.Frame.Height),
                    // ShowsStatistics = true, // Show stats
                };
 
                SetNativeControl(SceneView);

                SceneView.Delegate = new EyeGazeDetectionDelegate(SceneView);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe events
                // Pause the view's session
                SceneView.Session.Pause();
            }

            if (e.NewElement != null)
            {
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
                // Subscribe events
                // Run the view's session options
                SceneView.Session.Run(configuration,
                    ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: Stop sensor capturing
                //Control.CaptureSession.Dispose();
                Control.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ShowUnsupportedDeviceError()
        {
            var alertController = UIAlertController.Create("ARKit is not available on this device.",
                "This app requires world tracking, which is available only on iOS devices with the A9 processor or later.",
                UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.ViewController.PresentModalViewController(alertController, true);
        }
    }
}