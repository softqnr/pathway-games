using CoreML;
using Foundation;

namespace PathwayGames.iOS.Services.Engangement
{
    public class CoreMLPathwayInput : NSObject, IMLFeatureProvider
    {
        public double PPI { get; set; }
        public double Blink { get; set; }
        public double Squint { get; set; }
        public double GazeIn { get; set; }
        public double GazeOut { get; set; }
        public double Smile { get; set; }
        public double Frown { get; set; }
        public double HeadSpeed { get; set; }
        public double EyeDwell { get; set; }
        public double HeadTilt { get; set; }

        public NSSet<NSString> FeatureNames => new NSSet<NSString>(
            new NSString("ppi"),
            new NSString("eye_blink"),
            new NSString("eye_squint"),
            new NSString("eye_gaze_inward"),
            new NSString("eye_gaze_outward"),
            new NSString("smile"),
            new NSString("frown"),
            new NSString("head_speed"),
            new NSString("eye_dwelling"),
            new NSString("head_tilt"));

        public MLFeatureValue GetFeatureValue(string featureName)
        {
            switch (featureName)
            {
                case "ppi":
                    return MLFeatureValue.Create(PPI);
                case "eye_blink":
                    return MLFeatureValue.Create(Blink);
                case "eye_squint":
                    return MLFeatureValue.Create(Squint);
                case "eye_gaze_inward":
                    return MLFeatureValue.Create(GazeIn);
                case "eye_gaze_outward":
                    return MLFeatureValue.Create(GazeOut);
                case "smile":
                    return MLFeatureValue.Create(Smile);
                case "frown":
                    return MLFeatureValue.Create(Frown);
                case "head_speed":
                    return MLFeatureValue.Create(HeadSpeed);
                case "eye_dwelling":
                    return MLFeatureValue.Create(EyeDwell);
                case "head_tilt":
                    return MLFeatureValue.Create(HeadTilt);
                default:
                    return MLFeatureValue.Create(0); // default value
            }
        }
    }
}