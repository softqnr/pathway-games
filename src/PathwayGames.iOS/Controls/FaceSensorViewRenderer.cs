using ARKit;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using PathwayGames.Controls;
using PathwayGames.iOS.Controls;
using PathwayGames.iOS.Extensions;
using PathwayGames.Sensors;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FaceSensorView), typeof(FaceSensorViewRenderer))]
namespace PathwayGames.iOS.Controls
{
    public class FaceSensorViewRenderer : ViewRenderer<FaceSensorView, ARSCNView> , IARSessionDelegate
    {
        private IFaceSensor sensor;
        private ARSCNView SceneView;
        private bool IsTracked;


        protected override void OnElementChanged(ElementChangedEventArgs<FaceSensorView> e)
        {
            base.OnElementChanged(e);

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
                // Hide camera display
                SceneView.Scene.Background.Contents = UIColor.White;

                SetNativeControl(SceneView);

                SceneView.Session.Delegate = this;
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

                sensor = e.NewElement as IFaceSensor;
                // Subscribe events
                // Crosshair
                if (e.NewElement.ShowCrosshair)
                {
                    SceneView.Delegate = new EyeGazeDetectionDelegate(SceneView);
                }
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

        [Export("session:didUpdateFrame:")]
#pragma warning disable IDE0060 // Remove unused parameter
        public void DidUpdateFrame(ARSession session, ARFrame frame)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                foreach (ARAnchor anchor in frame.Anchors)
                {
                    if (anchor is ARFaceAnchor faceAnchor)
                    {
                        if (faceAnchor.IsTracked)
                        {
                            if (!IsTracked)
                            {
                                sensor.OnTrackingStarted(new EventArgs());
                                IsTracked = true;
                            }
                            // Log
                            sensor.OnReadingTaken(new FaceAnchorChangedEventArgs(
                                new FaceAnchorReading(frame.Timestamp, faceAnchor.Transform.ToFloatMatrix4(),
                                    faceAnchor.LeftEyeTransform.ToFloatMatrix4(),
                                    faceAnchor.RightEyeTransform.ToFloatMatrix4(),
                                    faceAnchor.LookAtPoint.ToFloatVector3(),
                                    faceAnchor.BlendShapes.ToDictionary()
                                )));
                        }else if (IsTracked)
                        {
                            sensor.OnTrackingStopped(new EventArgs());
                            IsTracked = false;
                        }
                    }
                }
                // Important otherwise frame will not be disposed
                frame.Dispose();
            });
        }
    }
}