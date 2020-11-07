using CoreML;
using Foundation;

namespace PathwayGames.iOS.Services
{
    public class CoreMLPathwayOutput : NSObject, IMLFeatureProvider
    {
        static readonly NSSet<NSString> featureNames = new NSSet<NSString>(
            new NSString("target"),
            new NSString("classProbability")
        );

        public double Target { get; set; }

        public double ClassProbability { get; set; }

        public NSSet<NSString> FeatureNames
        {
            get { return featureNames; }
        }

        public MLFeatureValue GetFeatureValue(string featureName)
        {
            switch (featureName)
            {
                case "target":
                    return MLFeatureValue.Create(Target);
                case "classProbability":
                    return MLFeatureValue.Create(ClassProbability);
                default:
                    return null;
            }
        }

        public CoreMLPathwayOutput(double price, double classProbability)
        {
            Target = price;
            ClassProbability = classProbability;
        }
    }
}