using ARKit;
using CoreFoundation;
using PathwayGames.Models;
using System;
using Xamarin.Forms;
using PathwayGames.iOS.Extensions;

namespace PathwayGames.iOS.Controls
{
    public class SessionDelegate : ARSessionDelegate
    {
        public SessionDelegate() { }

        public override void DidUpdateFrame(ARSession session, ARFrame frame)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                foreach (ARAnchor anchor in frame.Anchors){
                    var faceAnchor = anchor as ARFaceAnchor;
                    if (faceAnchor != null) {
                        //System.Diagnostics.Debug.WriteLine("Reading received: " + DateTime.Now);
                        // Log
                        MessagingCenter.Send<object, FaceAnchorData>(this, "EyeSensorReading",
                            new FaceAnchorData(frame.Timestamp, faceAnchor.Transform.ToFloatMatrix4(),
                                faceAnchor.LeftEyeTransform.ToFloatMatrix4(),
                                faceAnchor.RightEyeTransform.ToFloatMatrix4(),
                                faceAnchor.LookAtPoint.ToFloatVector3(),
                                faceAnchor.BlendShapes.ToDictionary()
                            ));
                   
                    }
                }
                // Important do not remove
                frame.Dispose();
            });
            
        }

        public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
        {
            var state = "";
            var reason = "";

            switch (camera.TrackingState)
            {
                case ARTrackingState.NotAvailable:
                    state = "Tracking Not Available";
                    break;
                case ARTrackingState.Normal:
                    state = "Tracking Normal";
                    break;
                case ARTrackingState.Limited:
                    state = "Tracking Limited";
                    switch (camera.TrackingStateReason)
                    {
                        case ARTrackingStateReason.ExcessiveMotion:
                            reason = "because of excessive motion";
                            break;
                        case ARTrackingStateReason.Initializing:
                            reason = "because tracking is initializing";
                            break;
                        case ARTrackingStateReason.InsufficientFeatures:
                            reason = "because of insufficient features in the environment";
                            break;
                        case ARTrackingStateReason.None:
                            reason = "because of an unknown reason";
                            break;
                    }
                    break;
            }

            System.Diagnostics.Debug.WriteLine("{0} {1}", state, reason);
        }
    }
}