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

        // actual physical size of device screen
        CGSize phoneScreenSize;
        // actual point size of device screen
        CGSize phoneScreenPointSize;
        CGSize compensation;

        nfloat screenScale = UIScreen.MainScreen.Scale;
        
        bool _recordingEnabled;
        bool _eyeGazeVisualizationEnabled;
        int _screenPPI;
        nfloat _widthCompensation;
        nfloat _heightCompensation;

        const int SmoothThreshold = 15;

        SCNNode virtualPhoneNode = new SCNNode();

        SCNNode virtualScreenNode;

        nfloat[] eyeLookAtPositionXs = new nfloat[SmoothThreshold];
        nfloat[] eyeLookAtPositionYs = new nfloat[SmoothThreshold];

        nfloat[] eyeLookAtPositionXsTmp = new nfloat[SmoothThreshold];
        nfloat[] eyeLookAtPositionYsTmp = new nfloat[SmoothThreshold];

        SCNHitTestOptions hitTestOptions = new SCNHitTestOptions
        {
            BackFaceCulling = false,
            SearchMode = SCNHitTestSearchMode.All,
            IgnoreChildNodes = false,
            IgnoreHiddenNodes = false,
        };

        public bool RecordingEnabled
        {
            get
            {
                return _recordingEnabled;
            }
            set
            {
                _recordingEnabled = value;
            }
        }

        public bool EyeGazeVisualizationEnabled
        {
            get
            {
                return _eyeGazeVisualizationEnabled;
            }
            set
            {
                _eyeGazeVisualizationEnabled = value;
                UpdateVisualization();
            }
        }

        public int ScreenPPI
        {
            get { 
                return _screenPPI; 
            }
            set { 
                _screenPPI = value; 
                UpdateScreenSize(); 
            }
        }

        public nfloat WidthCompensation
        {
            get
            {
                return _widthCompensation;
            }
            set
            {
                _widthCompensation = value;
                UpdateScreenSize();
            }
        }

        public nfloat HeightCompensation
        {
            get
            {
                return _heightCompensation;
            }
            set
            {
                _heightCompensation = value;
                UpdateScreenSize();
            }
        }

        public EyeGazeDetectionDelegate(ARSCNView sceneView, bool recordingEnabled, bool eyeGazeVisualizationEnabled, int screenPPI, float widthCompensantion, float heightCompensantion)
        {
            SceneView = sceneView;
            _screenPPI = screenPPI;
            _widthCompensation = widthCompensantion;
            _heightCompensation = heightCompensantion;
            _recordingEnabled = recordingEnabled;
            _eyeGazeVisualizationEnabled = eyeGazeVisualizationEnabled;

            // Hook to OrientantionChanged event
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIDeviceOrientationDidChangeNotification"), OrientantionChanged);

            // Virtual screen node
            CreateVirtualScreenNode();
            // Visualization
            CreateVisualizationNode();
            // Setup Scenegraph
            SetupScenegraph();

            UpdateVisualization();           
        }

        private void OrientantionChanged(NSNotification notification)
        {
            UpdateScreenSize();
        }

        private void UpdateVisualization()
        {
            // Hide visualization elements
            eyeLNode.Hidden = !_eyeGazeVisualizationEnabled;
            eyeRNode.Hidden = !_eyeGazeVisualizationEnabled;
            eyePositionIndicatorView.Hidden = !_eyeGazeVisualizationEnabled;
        }

        private void UpdateScreenSize()
        {
            switch (UIDevice.CurrentDevice.Orientation)
            {
                case UIDeviceOrientation.LandscapeLeft:
                case UIDeviceOrientation.LandscapeRight:
                    //phoneScreenSize = new CGSize(0.14351, 0.070866);
                    phoneScreenPointSize = new CGSize(UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width);
                    phoneScreenSize = new CGSize(((phoneScreenPointSize.Height * screenScale) / _screenPPI) * 2.54 / 100,
                        ((phoneScreenPointSize.Width * screenScale) / _screenPPI) * 2.54 / 100);
                    compensation = new CGSize(_heightCompensation, _widthCompensation);
                    break;
                default:
                    //phoneScreenSize = new CGSize(0.070866, 0.14351);
                    phoneScreenPointSize = new CGSize(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
                    phoneScreenSize = new CGSize(((phoneScreenPointSize.Width * screenScale) / _screenPPI) * 2.54 / 100,
                        ((phoneScreenPointSize.Height * screenScale) / _screenPPI) * 2.54 / 100);
                    compensation = new CGSize(_widthCompensation, _heightCompensation);
                    break;
            }
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
            if (!anchor.IsTracked || !RecordingEnabled || ScreenPPI == 0)
                return;

            // Render eye rays
            eyeRNode.Transform = anchor.RightEyeTransform.ToSCNMatrix4();
            eyeLNode.Transform = anchor.LeftEyeTransform.ToSCNMatrix4();

            // Perform Hit test using the ray segments that are drawn by the center of the eyeballs 
            // to somewhere two meters away at direction of where users look at to the virtual plane that place 
            // at the same orientation of the phone screen
            var phoneScreenEyeRHitTestResults = virtualPhoneNode.HitTest(lookAtTargetEyeRNode.WorldPosition, 
                eyeRNode.WorldPosition, hitTestOptions);
            
            // HitTest with segment (virtualPhoneNode)
            var phoneScreenEyeLHitTestResults = virtualPhoneNode.HitTest(lookAtTargetEyeLNode.WorldPosition, 
                eyeLNode.WorldPosition, hitTestOptions);
            
            if (phoneScreenEyeLHitTestResults.Length > 0 && phoneScreenEyeRHitTestResults.Length > 0)
            {
                CGPoint coordinates = ScreenPositionFromHittest(phoneScreenEyeLHitTestResults[0], phoneScreenEyeRHitTestResults[0]);
               
                // Update indicator position ? - SceneView.Superview.Frame.Width
                DispatchQueue.MainQueue.DispatchAsync(() => {
                    //eyePositionIndicatorView.Center = coordinates;
                    eyePositionIndicatorView.Transform = CGAffineTransform.MakeTranslation(
                        coordinates.X,
                        coordinates.Y
                    );

                    //var distanceInCm = CalculateDistanceFromCamera();
                });

                // Calc eye screen position 
                //int screenX = (int)(coordinates.X * screenScale);
                //int screenY = (int)(coordinates.Y * screenScale);
                //System.Diagnostics.Debug.WriteLine($"Screen X: {screenX} - Y: {screenY}");

                // Send sensor data 
                //MessagingCenter.Send<object, EyeGazeData>(this, "EyeSensorReading",  
                //    new EyeGazeData(DateTime.Now, screenX, screenY, distanceInCm));
            }
        }

        private CGPoint ScreenPositionFromHittest(SCNHitTestResult phoneScreenEyeLHitTestResult, SCNHitTestResult  phoneScreenEyeRHitTestResult)
        {
            CGPoint eyeLLookAt = new CGPoint();
            CGPoint eyeRLookAt = new CGPoint();

            eyeRLookAt.X = (nfloat)phoneScreenEyeRHitTestResult.LocalCoordinates.X / (phoneScreenSize.Width / 2) * phoneScreenPointSize.Width + compensation.Width;
            eyeRLookAt.Y = (nfloat)phoneScreenEyeRHitTestResult.LocalCoordinates.Y / (phoneScreenSize.Height / 2) * (phoneScreenPointSize.Height + compensation.Height);

            eyeLLookAt.X = (nfloat)phoneScreenEyeLHitTestResult.LocalCoordinates.X / (phoneScreenSize.Width / 2) * phoneScreenPointSize.Width + compensation.Width;
            eyeLLookAt.Y = (nfloat)phoneScreenEyeLHitTestResult.LocalCoordinates.Y / (phoneScreenSize.Height / 2) * (phoneScreenPointSize.Height + compensation.Height);

            // Smooth
            // Add the latest position and keep up to 9 recent position to smooth with.
            // Add it to first array position
            eyeLookAtPositionXs[0] = ((eyeRLookAt.X + eyeLLookAt.X) / 2);
            eyeLookAtPositionYs[0] = (-(eyeRLookAt.Y + eyeLLookAt.Y) / 2);
            // Copy rest of readings 
            Array.Copy(eyeLookAtPositionXsTmp, 0, eyeLookAtPositionXs, 1, SmoothThreshold - 1);
            Array.Copy(eyeLookAtPositionYsTmp, 0, eyeLookAtPositionYs, 1, SmoothThreshold - 1);
            // Cache readings
            eyeLookAtPositionXs.CopyTo(eyeLookAtPositionXsTmp, 0);
            eyeLookAtPositionYs.CopyTo(eyeLookAtPositionYsTmp, 0);

            // Calc average
            float smoothEyeLookAtPositionX = eyeLookAtPositionXs.Average();
            float smoothEyeLookAtPositionY = eyeLookAtPositionYs.Average();
 
            return new CGPoint(smoothEyeLookAtPositionX, smoothEyeLookAtPositionY);
        }

        private double CalculateDistanceFromCamera()
        {
            // Calculate distance of the eyes to the camera
            var distanceL = eyeLNode.WorldPosition - SCNVector3.Zero;
            var distanceR = eyeRNode.WorldPosition - SCNVector3.Zero;

            // Average distance from two eyes
            var distance = (distanceL.Length() + distanceR.Length()) / 2;

            var distanceInCm = Math.Round(distance * 100, MidpointRounding.AwayFromZero);

            return distanceInCm;
        }

        public override void Update(ISCNSceneRenderer renderer, double timeInSeconds)
        {
            // Check null point of view
            virtualPhoneNode.Transform = SceneView.PointOfView.Transform;
        }

        private void CreateVisualizationNode()
        {
            // Create target    
            eyePositionIndicatorView.Bounds = new CGRect(0, 0, 12, 12);
            eyePositionIndicatorView.Layer.CornerRadius = eyePositionIndicatorView.Bounds.Width / 2;
            eyePositionIndicatorView.BackgroundColor = UIColor.Red;
            eyePositionIndicatorView.Center = new CGPoint(SceneView.Superview.Frame.Size.Width / 2,
                       SceneView.Superview.Frame.Size.Height / 2);

            eyePositionIndicatorView.Hidden = !_eyeGazeVisualizationEnabled;
            // Add eye position indicator to parent view
            SceneView.Superview.AddSubview(eyePositionIndicatorView);
        }

        private void CreateVirtualScreenNode()
        {
            SCNPlane screenGeometry = new SCNPlane
            {
                Width = 1,
                Height = 1
            };

            screenGeometry.FirstMaterial.DoubleSided = true;
            screenGeometry.FirstMaterial.Diffuse.Contents = UIColor.Green;

            virtualScreenNode = SCNNode.FromGeometry(screenGeometry);
        }

        private void SetupScenegraph()
        {
            eyeLNode = CreateEyeNode();
            eyeRNode = CreateEyeNode();

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