using ARKit;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using PathwayGames.Controls;
using PathwayGames.Infrastructure.Timer;
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
        private NSObject CameraPreview;
        
        protected override void OnElementChanged(ElementChangedEventArgs<FaceSensorView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe events
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
            }

            if (e.NewElement != null)
            {
                // Check if AR is supported
                if (!ARFaceTrackingConfiguration.IsSupported)
                {
                    ShowUnsupportedDeviceError();
                    return;
                }
                if (Control == null)
                {
                    // Instantiate the native control and assign it to the Control property with
                    // the SetNativeControl method
                    SceneView = new ARSCNView
                    {
                        ContentMode = UIViewContentMode.ScaleToFill,
                        UserInteractionEnabled = true,
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        AutomaticallyUpdatesLighting = true,
                        Frame = new CGRect(Frame.X, Frame.Y, Frame.Width, Frame.Height),
                        // UITapGestureRecognizer = new UITapGestureRecognizer(),
                        // ShowsStatistics = true, // Show stats
                    };

                    SetNativeControl(SceneView);

                    SceneView.Session.Delegate = this;

                    SceneView.Delegate = new EyeGazeDetectionDelegate(SceneView,
                        e.NewElement.RecordingEnabled,
                        e.NewElement.EyeGazeVisualizationEnabled,
                        e.NewElement.ScreenPPI,
                        e.NewElement.WidthCompensation,
                        e.NewElement.HeightCompensation
                        );
                }

                sensor = e.NewElement as IFaceSensor;
                // Subscribe events
                e.NewElement.PropertyChanged += OnElementPropertyChanged;
                // Begin AR Session
                SceneView.Session.Run(new ARFaceTrackingConfiguration
                {
                    LightEstimationEnabled = true
                }, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (this.Element == null || this.Control == null)
                return;

            if (e.PropertyName == FaceSensorView.RecordingEnabledProperty.PropertyName) {
                UpdateRecordingEnabled();
            } else if (e.PropertyName == FaceSensorView.EyeGazeVisualizationEnabledProperty.PropertyName) {
                UpdateEyeGazeVisualization();
            } else if (e.PropertyName == FaceSensorView.ScreenPPIProperty.PropertyName) {
                UpdateScreenPPI();
            } else if (e.PropertyName == FaceSensorView.CameraPreviewEnabledProperty.PropertyName) {
                UpdateCameraPreview();
            } else if (e.PropertyName == FaceSensorView.WidthCompensationProperty.PropertyName) {
                UpdatWidthCompensation();
            } else if (e.PropertyName == FaceSensorView.HeightCompensationProperty.PropertyName) {
                UpdatHeightCompensation();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Pause the AR session
                SceneView.Session?.Pause();
                Control.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateRecordingEnabled()
        {
            (SceneView.Delegate as EyeGazeDetectionDelegate).RecordingEnabled = sensor.RecordingEnabled;
        }

        private void UpdateEyeGazeVisualization()
        {
            (SceneView.Delegate as EyeGazeDetectionDelegate).EyeGazeVisualizationEnabled = sensor.EyeGazeVisualizationEnabled;
        }

        private void UpdateScreenPPI()
        {
            (SceneView.Delegate as EyeGazeDetectionDelegate).ScreenPPI = sensor.ScreenPPI;
        }

        private void UpdateCameraPreview()
        {
            if (CameraPreview == null)
            {
                CameraPreview = SceneView.Scene.Background.Contents;
            }
            if (sensor.EyeGazeVisualizationEnabled)
            {
                SceneView.Scene.Background.Contents = CameraPreview;
            }
            else
            {
                SceneView.Scene.Background.Contents = UIColor.Clear;
            }
        }

        private void UpdatWidthCompensation()
        {
            (SceneView.Delegate as EyeGazeDetectionDelegate).WidthCompensation = sensor.WidthCompensation;
        }

        private void UpdatHeightCompensation()
        {
            (SceneView.Delegate as EyeGazeDetectionDelegate).HeightCompensation = sensor.HeightCompensation;
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
                                new FaceAnchorReading(TimerClock.Now, 
                                    faceAnchor.Transform.ToFloatMatrix4(),
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