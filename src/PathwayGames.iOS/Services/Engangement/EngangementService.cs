using CoreML;
using Foundation;
using MathNet.Numerics.Statistics;

using PathwayGames.iOS.Extensions;
using PathwayGames.iOS.Services.Engangement;
using PathwayGames.Sensors;
using PathwayGames.Services.Engangement;
using PathwayGames.Extensions;
using System;
using System.Drawing;
using PathwayGames.Models.Enums;

[assembly: Xamarin.Forms.Dependency(typeof(EngangementService))]
namespace PathwayGames.iOS.Services.Engangement
{
    public class EngangementService : IEngagementService
    {
        public int SampleWindow = 10;

        private float PPI;
        private MovingStatistics blink;
        private MovingStatistics squint;
        private MovingStatistics gazeIn;
        private MovingStatistics gazeOut;
        private MovingStatistics smile;
        private MovingStatistics frown;
        private MovingStatistics headSpeed;
        private MovingStatistics eyeDwell;
        private MovingStatistics headTilt;
        private FaceAnchorReading previousReading;
        private MLModel model;
        private float value;

        public EngangementService()
        {
            LoadModel();
        }

        public void StartSession(float ppi, int sensitivity)
        {
            PPI = ppi;

            var sampleWindow = sensitivity * 60; // 60 to 1200ms, at 60 frames/sec, = 1 sec to 20 seconds

            blink = new MovingStatistics(sampleWindow);
            squint = new MovingStatistics(sampleWindow);
            gazeIn = new MovingStatistics(sampleWindow);
            gazeOut = new MovingStatistics(sampleWindow);
            smile = new MovingStatistics(sampleWindow);
            frown = new MovingStatistics(sampleWindow);
            headSpeed = new MovingStatistics(sampleWindow);
            eyeDwell = new MovingStatistics(sampleWindow);
            headTilt = new MovingStatistics(sampleWindow);
        }

        private void LoadModel()
        {
            var modelPath = NSBundle.MainBundle.GetUrlForResource("CoreMLModel/rf_latest_80", "mlmodelc");
            model = MLModel.Create(modelPath, out _);
        }

        //public static string GetAbsolutePath(string relativePath)
        //{
        //    FileInfo _dataRoot = new FileInfo(typeof(App).Assembly.Location);
        //    string assemblyFolderPath = _dataRoot.Directory.FullName;

        //    string fullPath = Path.Combine(assemblyFolderPath, relativePath);

        //    return fullPath;
        //}

        public Color GetEngagementColor(Tolerance tolerance)
        {
            Models.Engangement engangemet = new Models.Engangement(value, tolerance);

            // Blend the two range colors
            return engangemet.Color2.Blend(engangemet.Color1, engangemet.Delta);
        }

        public double GetEngagement(FaceAnchorReading faceAnchorReading)
        {
            return UpdateEngagement(faceAnchorReading);
        }

        private double[] CoordinateDisplacement(float[,] coordinate)
        {
            var rotational_disp = 0d;
            var transitional_disp = 0d;

            if (coordinate.Length == 16)
            {
                var x_rot_disp = VectorDifference(coordinate.GetRow(0), new float[] { 1, 0, 0 });
                var y_rot_disp = VectorDifference(coordinate.GetRow(1), new float[] { 0, 1, 0 });
                var z_rot_disp = VectorDifference(coordinate.GetRow(2), new float[] { 0, 0, 1 });
                rotational_disp = VectorMagnitude(AddVectors(x_rot_disp, y_rot_disp, z_rot_disp));
                transitional_disp = VectorMagnitude(VectorDifference(coordinate.GetRow(3), new float[] { 0, 0, 0 }));
            }

            return new[] { transitional_disp, rotational_disp };
        }

        private double[] VectorDifference(float[] a_array, float[] b_array)
        {
            return new double[] { b_array[0] - a_array[0], b_array[1] - a_array[1], b_array[2] - a_array[2] };
        }

        private double VectorMagnitude(double[] vec_array)
        {
            return Math.Pow(Math.Pow(vec_array[0], 2) + Math.Pow(vec_array[1], 2) + Math.Pow(vec_array[2], 2), 0.5);
        }

        private double[] AddVectors(double[] a_array, double[] b_array, double[] c_array)
        {
            if (a_array.Length + b_array.Length + c_array.Length == 9)
                return new double[] { a_array[0] + b_array[0] + c_array[0], a_array[1] + b_array[1] + c_array[1], a_array[2] + b_array[2] + c_array[2] };
            else
                return new double[] { 0, 0, 0 };
        }

        public double UpdateEngagement(FaceAnchorReading f)
        {
            blink.Push((f.FacialExpressions["EyeBlinkLeft"].Value + f.FacialExpressions["EyeBlinkRight"].Value) / 2);
            smile.Push((f.FacialExpressions["MouthSmileLeft"].Value + f.FacialExpressions["MouthSmileRight"].Value) / 2);
            frown.Push((f.FacialExpressions["MouthFrownLeft"].Value + f.FacialExpressions["MouthFrownRight"].Value) / 2);
            squint.Push((f.FacialExpressions["EyeSquintLeft"].Value + f.FacialExpressions["EyeSquintRight"].Value) / 2);
            gazeIn.Push((f.FacialExpressions["EyeLookInLeft"].Value + f.FacialExpressions["EyeLookInRight"].Value) / 2);
            gazeOut.Push((f.FacialExpressions["EyeLookOutLeft"].Value + f.FacialExpressions["EyeLookOutRight"].Value) / 2);

            if (previousReading == null)
            {
                headSpeed.Push(0);
                eyeDwell.Push(0);
                headTilt.Push(0);
            }
            else
            {
                var deltaTime = f.ReadingTimestamp.Subtract(previousReading.ReadingTimestamp);
                var deltaHead = VectorMagnitude(VectorDifference(previousReading.LookAtPointTransform, f.LookAtPointTransform));
                headSpeed.Push((float)(deltaHead / deltaTime.TotalMilliseconds));

                var leftEyeDisplacement = CoordinateDisplacement(f.LeftEyeTransform);
                var rightEyeDisplacement = CoordinateDisplacement(f.RightEyeTransform);
                var eyeDisplacement = Math.Abs(leftEyeDisplacement[0] + rightEyeDisplacement[0]) / 2;
                eyeDwell.Push((float)(eyeDisplacement > 0 ? 1 / eyeDisplacement : 0));

                headTilt.Push((float)(Math.Abs(leftEyeDisplacement[1] + rightEyeDisplacement[1]) / 2));
            }

            previousReading = f;

            var modelInput = new CoreMLPathwayInput
            {
                PPI = PPI,
                Blink = blink.Mean,
                Smile = smile.Mean,
                Frown = frown.Mean,
                Squint = squint.Mean,
                GazeIn = gazeIn.Mean,
                GazeOut = gazeOut.Mean,
                HeadSpeed = headSpeed.Mean,
                EyeDwell = eyeDwell.Mean,
                HeadTilt = headTilt.Mean
            };
            IMLFeatureProvider predictionOut = model.GetPrediction(modelInput, out _);

            var targetFeatureValue = predictionOut.GetFeatureValue("target");
            var prediction = targetFeatureValue.Int64Value;

            var classProbabilityFeatureValue = predictionOut.GetFeatureValue("classProbability");
            var probabilityx = classProbabilityFeatureValue.DictionaryValue.Values[0];
            var probabilityy = classProbabilityFeatureValue.DictionaryValue.Values[1];

            //// this is a bit hacky, but it's all relative
            //value = (probabilityy.FloatValue - probabilityx.FloatValue - 0.5f) * 2f;
            //value = (value < 0f) ? 0f : value;

            value = probabilityy.FloatValue - probabilityx.FloatValue;

            if (value < 0f)
            {
                Console.WriteLine(value + " negative !");
                value = 0f;
            }
            else
                Console.WriteLine(value);

            return value;
            //return probabilityy.FloatValue;
            //return prediction;
        }

        //var noms = new[] {
        //            "BrowDownLeft", "BrowDownRight", "BrowInnerUp", "BrowOuterUpLeft",
        //            "BrowOuterUpRight", "CheekPuff", "CheekSquintLeft", "CheekSquintRight",
        //            "EyeBlinkLeft", "EyeBlinkRight", "EyeLookDownLeft", "EyeLookDownRight",
        //            "EyeLookInLeft", "EyeLookInRight", "EyeLookOutLeft", "EyeLookOutRight",
        //            "EyeLookUpLeft", "EyeLookUpRight", "EyeSquintLeft", "EyeSquintRight",
        //            "EyeWideLeft", "EyeWideRight",
        //            "JawForward", "JawLeft", "JawOpen", "JawRight",
        //            "MouthClose", "MouthDimpleLeft", "MouthDimpleRight", "MouthFrownLeft", "MouthFrownRight",
        //            "MouthFunnel", "MouthLeft", "MouthLowerDownLeft", "MouthLowerDownRight", "MouthPressLeft",
        //            "MouthPressRight", "MouthPucker", "MouthRight", "MouthRollLower", "MouthRollUpper",
        //            "MouthShrugLower", "MouthShrugUpper", "MouthSmileLeft", "MouthSmileRight", "MouthStretchLeft",
        //            "MouthStretchRight", "MouthUpperUpLeft", "MouthUpperUpRight", "NoseSneerLeft", "NoseSneerRight",
        //            "TongueOut" };

        //// This file was auto-generated by ML.NET Model Builder. 
        //public class ModelInput
        //{
        //    [ColumnName("target"), LoadColumn(0)]
        //    public bool Target { get; set; }

        //    [ColumnName("org"), LoadColumn(1)]
        //    public string Org { get; set; }

        //    [ColumnName("userid"), LoadColumn(2)]
        //    public float Userid { get; set; }

        //    [ColumnName("user_name"), LoadColumn(3)]
        //    public string User_name { get; set; }

        //    [ColumnName("date"), LoadColumn(4)]
        //    public float Date { get; set; }

        //    [ColumnName("slideCount"), LoadColumn(5)]
        //    public float SlideCount { get; set; }

        //    [ColumnName("ppi"), LoadColumn(6)]
        //    public float Ppi { get; set; }

        //    [ColumnName("press_count"), LoadColumn(7)]
        //    public float Press_count { get; set; }

        //    [ColumnName("response_time"), LoadColumn(8)]
        //    public float Response_time { get; set; }

        //    [ColumnName("eye_blink"), LoadColumn(9)]
        //    public float Blink { get; set; }

        //    [ColumnName("eye_squint"), LoadColumn(10)]
        //    public float Squint { get; set; }

        //    [ColumnName("eye_gaze_inward"), LoadColumn(11)]
        //    public float GazeIn { get; set; }

        //    [ColumnName("eye_gaze_outward"), LoadColumn(12)]
        //    public float GazeOut { get; set; }

        //    [ColumnName("smile"), LoadColumn(13)]
        //    public float Smile { get; set; }

        //    [ColumnName("frown"), LoadColumn(14)]
        //    public float Frown { get; set; }

        //    [ColumnName("head_speed"), LoadColumn(15)]
        //    public float HeadSpeed { get; set; }

        //    [ColumnName("eye_dwelling"), LoadColumn(16)]
        //    public float EyeDwell { get; set; }

        //    [ColumnName("head_tilt"), LoadColumn(17)]
        //    public float HeadTilt { get; set; }

        //}

        //public class ModelOutput
        //{
        //    // ColumnName attribute is used to change the column name from
        //    // its default value, which is the name of the field.
        //    [ColumnName("PredictedLabel")]
        //    public bool Prediction { get; set; }

        //    public float Score { get; set; }
        //}
    }
}