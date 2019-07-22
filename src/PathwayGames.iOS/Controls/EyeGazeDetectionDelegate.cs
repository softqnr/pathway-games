using ARKit;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using PathwayGames.iOS.Extensions;
using SceneKit;
using System;
using System.Linq;
using UIKit;

namespace PathwayGames.iOS.Controls
{
    public class EyeGazeDetectionDelegate : ARSCNViewDelegate
    {
        ARSCNView SceneView;
        SCNNode faceNode = new SCNNode();

        SCNNode eyeLNode;
        SCNNode eyeRNode;

        UIView eyePositionIndicatorView = new UIView();

        SCNNode lookAtTargetEyeLNode = new SCNNode();
        SCNNode lookAtTargetEyeRNode = new SCNNode();

        // actual physical size of iPhoneX screen
        //CGSize phoneScreenSize = new CGSize(0.0623908297, 0.135096943231532);
        CGSize phoneScreenSize = new CGSize(0.070866, 0.14351);
        // actual point size of iPhoneX screen
        CGSize phoneScreenPointSize = new CGSize(375, 812);

        const int SmoothThresholdNumber = 15;

        SCNNode virtualPhoneNode = new SCNNode();

        SCNNode virtualScreenNode;

        nfloat[] eyeLookAtPositionXs = new nfloat[SmoothThresholdNumber];
        nfloat[] eyeLookAtPositionYs = new nfloat[SmoothThresholdNumber];

        nfloat[] eyeLookAtPositionXsTmp = new nfloat[SmoothThresholdNumber];
        nfloat[] eyeLookAtPositionYsTmp = new nfloat[SmoothThresholdNumber];

        public EyeGazeDetectionDelegate(ARSCNView sceneView)
        {
            SceneView = sceneView;
            //sceneView.layer.CornerRadius = 28
            // Virtual screen node
            CreateVirtualScreenNode();
            // Create target    
            eyePositionIndicatorView.Bounds = new CGRect(0, 0, 12, 12);//me
            eyePositionIndicatorView.Layer.CornerRadius = eyePositionIndicatorView.Bounds.Width / 2;
            eyePositionIndicatorView.BackgroundColor = UIColor.Red;//me
            eyePositionIndicatorView.Center = new CGPoint(SceneView.Superview.Frame.Size.Width / 2,
                       SceneView.Superview.Frame.Size.Height / 2);

            // Setup Scenegraph
            eyeLNode = CreateEyeNode();
            eyeRNode = CreateEyeNode();

            // Add eye position indicator to parent view
            SceneView.Superview.AddSubview(eyePositionIndicatorView);
       
            SceneView.Scene.RootNode.AddChildNode(faceNode);
            SceneView.Scene.RootNode.AddChildNode(virtualPhoneNode);
            virtualPhoneNode.AddChildNode(virtualScreenNode);
            faceNode.AddChildNode(eyeLNode);
            faceNode.AddChildNode(eyeRNode);
            eyeLNode.AddChildNode(lookAtTargetEyeLNode);
            eyeRNode.AddChildNode(lookAtTargetEyeRNode);

            // Set LookAtTargetEye at 2 meters away from the center of eyeballs to create segment vector
            lookAtTargetEyeLNode.Position = new SCNVector3(lookAtTargetEyeLNode.Position.X, lookAtTargetEyeLNode.Position.Y, 2);

            lookAtTargetEyeRNode.Position = new SCNVector3(lookAtTargetEyeRNode.Position.X, lookAtTargetEyeRNode.Position.Y, 2);
        }

        public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            // Check We Have A Valid ARFaceAnchor
            if (anchor != null && anchor is ARFaceAnchor faceAnchor)
            {
                faceNode.Transform = node.Transform;
                Update(faceAnchor);
            }
        }

        public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            // Check We Have A Valid ARFaceAnchor
            if (anchor != null && anchor is ARFaceAnchor faceAnchor)
            {
                faceNode.Transform = node.Transform;
                Update(faceAnchor);
            }
        }

        public void Update(ARFaceAnchor anchor)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => {
                // Render eye rays
                eyeRNode.Transform = anchor.RightEyeTransform.ToSCNMatrix4();
                eyeLNode.Transform = anchor.LeftEyeTransform.ToSCNMatrix4();

                CGPoint eyeLLookAt = new CGPoint();
                CGPoint eyeRLookAt = new CGPoint();

                nfloat heightCompensation = 312;
                // Perform Hit test using the ray segments that are drawn by the center of the eyeballs 
                // to somewhere two meters away at direction of where users look at to the virtual plane that place 
                // at the same orientation of the phone screen
                var phoneScreenEyeRHitTestResults = virtualPhoneNode.HitTest(lookAtTargetEyeRNode.WorldPosition, 
                    eyeRNode.WorldPosition, (NSDictionary)null);
                // HitTest with segment (virtualPhoneNode)
                var phoneScreenEyeLHitTestResults = virtualPhoneNode.HitTest(lookAtTargetEyeLNode.WorldPosition, 
                    eyeLNode.WorldPosition, (NSDictionary)null);

                foreach (var result in phoneScreenEyeRHitTestResults)
                {
                    eyeRLookAt.X = (nfloat)result.LocalCoordinates.X / (phoneScreenSize.Width / 2) * phoneScreenPointSize.Width;

                    eyeRLookAt.Y = (nfloat)result.LocalCoordinates.Y / (phoneScreenSize.Height / 2) * (phoneScreenPointSize.Height + heightCompensation);
                }

                foreach (var result in phoneScreenEyeLHitTestResults)
                {
                    eyeLLookAt.X = (nfloat)result.LocalCoordinates.X / (phoneScreenSize.Width / 2) * phoneScreenPointSize.Width;

                    eyeLLookAt.Y = (nfloat)result.LocalCoordinates.Y / (phoneScreenSize.Height / 2) * (phoneScreenPointSize.Height + heightCompensation);
                }

                //Console.WriteLine($"Look Left X: {eyeLLookAt.X} - Y: {eyeLLookAt.Y} - Right X: {eyeRLookAt.X} - Y: {eyeRLookAt.Y}");
                // Add the latest position and keep up to 9 recent position to smooth with.
                // Add it to first array position
                eyeLookAtPositionXs[0] = ((eyeRLookAt.X + eyeLLookAt.X) / 2);
                eyeLookAtPositionYs[0] = (-(eyeRLookAt.Y + eyeLLookAt.Y) / 2);
                // Copy rest of readings 
                Array.Copy(eyeLookAtPositionXsTmp, 0, eyeLookAtPositionXs, 1, SmoothThresholdNumber - 1);
                Array.Copy(eyeLookAtPositionYsTmp, 0, eyeLookAtPositionYs, 1, SmoothThresholdNumber - 1);
                // Cache readings
                eyeLookAtPositionXs.CopyTo(eyeLookAtPositionXsTmp, 0);
                eyeLookAtPositionYs.CopyTo(eyeLookAtPositionYsTmp, 0);

                // Calc average
                float smoothEyeLookAtPositionX = eyeLookAtPositionXs.Average();
                float smoothEyeLookAtPositionY = eyeLookAtPositionYs.Average();

                // Update indicator position ? - SceneView.Superview.Frame.Width
                eyePositionIndicatorView.Transform = CGAffineTransform.MakeTranslation(
                    smoothEyeLookAtPositionX / 2, 
                    smoothEyeLookAtPositionY / 2
                );

                // Calc eye screen position 
                int screenX = (int)(smoothEyeLookAtPositionX + phoneScreenPointSize.Width / 2);
                int screenY = (int)(smoothEyeLookAtPositionY + phoneScreenPointSize.Height / 2);

                // Calculate distance of the eyes to the camera
                var distanceL = eyeLNode.WorldPosition - SCNVector3.Zero;
                var distanceR = eyeRNode.WorldPosition - SCNVector3.Zero;

                // Average distance from two eyes
                var distance = (distanceL.Length() + distanceR.Length()) / 2;

                var distanceInCm = Math.Round(distance * 100, MidpointRounding.AwayFromZero);
                
                //System.Diagnostics.Debug.WriteLine($"Screen X: {screenX} - Y: {screenY} - Distance: {distanceInCm}cm");
            });
        }

        public override void Update(ISCNSceneRenderer renderer, double timeInSeconds)
        {
            // Check null point of view
            virtualPhoneNode.Transform = SceneView.PointOfView.Transform;
        }

        private void CreateVirtualScreenNode()
        {
            SCNPlane screenGeometry = new SCNPlane
            {
                Width = 1,
                Height = 1
            };

            // TODO: FirstMaterial check for null
            screenGeometry.FirstMaterial.DoubleSided = true;
            screenGeometry.FirstMaterial.Diffuse.Contents = UIColor.Green;

            virtualScreenNode = SCNNode.FromGeometry(screenGeometry);
        }

        private SCNNode CreateEyeNode()
        {
            // Create A Node To Represent The Eye
            var eyeGeometry = SCNCone.Create(0.005f, 0, 0.2f);
            eyeGeometry.RadialSegmentCount = 3;
            eyeGeometry.FirstMaterial.Diffuse.Contents = UIColor.Green;
            //eyeGeometry.FirstMaterial.Transparency = 0.5f;

            // Create A Holder Node & Rotate It So The Gemoetry Points Towards The Device
            SCNNode node = SCNNode.FromGeometry(eyeGeometry);

            node.EulerAngles = new SCNVector3((float)-Math.PI / 2, node.EulerAngles.Y, node.EulerAngles.Z);

            node.Position = new SCNVector3(node.Position.X, node.Position.Y, 0.1f);

            SCNNode parentNode = new SCNNode();
            parentNode.AddChildNode(node);

            return parentNode;
        }
    }
}