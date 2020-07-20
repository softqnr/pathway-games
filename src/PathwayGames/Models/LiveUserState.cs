//using DocumentFormat.OpenXml.Bibliography;
//using DocumentFormat.OpenXml.Drawing;
//using DocumentFormat.OpenXml.ExtendedProperties;
//using MathNet.Numerics.LinearAlgebra.Complex.Solvers;
//using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using PathwayGames.Sensors;
using PathwayGamesML.Model;
using System;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using Xamarin.Forms;

namespace PathwayGames.Models
{
    public class LiveUserState
    {
        public int SampleWindow = 20;

        private MovingStatistics blink;
        private MovingStatistics squint;
        private MovingStatistics gazeIn;
        private MovingStatistics gazeOut;
        private MovingStatistics smile;
        private MovingStatistics frown;
        private MovingStatistics headSpeed;
        private MovingStatistics eyeDwell;
        private MovingStatistics headTilt;
        //private MovingStatistics pressCount;
        //private MovingStatistics responseTime;

        private FaceAnchorReading previousReading;

        private MLContext mlContext;
        private DataViewSchema modelSchema;
        private ITransformer trainedModel;
        private PredictionEngine<ModelInput, ModelOutput> predictionEngine;

        public LiveUserState()
        {
            LoadModel();

            blink = new MovingStatistics(SampleWindow);
            squint = new MovingStatistics(SampleWindow);
            gazeIn = new MovingStatistics(SampleWindow);
            gazeOut = new MovingStatistics(SampleWindow);
            smile = new MovingStatistics(SampleWindow);
            frown = new MovingStatistics(SampleWindow);
            headSpeed = new MovingStatistics(SampleWindow);
            eyeDwell = new MovingStatistics(SampleWindow);
            headTilt = new MovingStatistics(SampleWindow);
            //pressCount = new MovingStatistics(TimeWindow);
            //responseTime = new MovingStatistics(TimeWindow);
        }

        private void LoadModel()
        {
            mlContext = new MLContext();
            string path = GetAbsolutePath(@"MLModel.zip");
            var model = mlContext.Model;
            trainedModel = model.Load(path, out modelSchema);
            predictionEngine = model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

        }
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(App).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        public double? GetState(FaceAnchorReading faceAnchorReading)
        {
            UpdateEngagement(faceAnchorReading);

            return null;
        }

        private double[] CoordinateDisplacement(float[,] coordinate)
        {
            var rotational_disp = 0d;
            var transitional_disp = 0d;

            if (coordinate.Length == 16)
            {
                var x_rot_disp = VectorDifference(coordinate.GetRow(0), new float[] { 1, 0, 0 });
                var y_rot_disp = VectorDifference(coordinate.GetRow(1), new float [] { 0, 1, 0 });
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
            var modelInput = new ModelInput();
            modelInput.Blink = (f.FacialExpressions["EyeBlinkLeft"].Value + f.FacialExpressions["EyeBlinkRight"].Value) / 2;
            modelInput.Smile = (f.FacialExpressions["MouthSmileLeft"].Value + f.FacialExpressions["MouthSmileRight"].Value) / 2;
            modelInput.Frown = (f.FacialExpressions["MouthFrownLeft"].Value + f.FacialExpressions["MouthFrownRight"].Value) / 2;
            modelInput.Squint = (f.FacialExpressions["EyeSquintLeft"].Value + f.FacialExpressions["EyeSquintRight"].Value) / 2;
            modelInput.GazeIn = (f.FacialExpressions["EyeLookInLeft"].Value + f.FacialExpressions["EyeLookInRight"].Value) / 2;
            modelInput.GazeOut = (f.FacialExpressions["EyeLookOutLeft"].Value + f.FacialExpressions["EyeLookOutRight"].Value) / 2;

            if (previousReading == null)
            {
                modelInput.HeadSpeed = 0;
                modelInput.EyeDwell = 0;
                modelInput.HeadTilt = 0;
            }
            else
            {
                var deltaTime = f.ReadingTimestamp.Subtract(previousReading.ReadingTimestamp);
                var deltaHead = VectorMagnitude(VectorDifference(previousReading.LookAtPointTransform, f.LookAtPointTransform));
                modelInput.HeadSpeed = (float) (deltaHead / deltaTime.TotalMilliseconds);

                var leftEyeDisplacement = CoordinateDisplacement(f.LeftEyeTransform);
                var rightEyeDisplacement = CoordinateDisplacement(f.RightEyeTransform);
                var eyeDisplacement = Math.Abs(leftEyeDisplacement[0] + rightEyeDisplacement[0]) / 2;
                modelInput.EyeDwell = (float) (eyeDisplacement > 0 ? 1 / eyeDisplacement : 0);

                modelInput.HeadTilt = (float) (Math.Abs(leftEyeDisplacement[1] + rightEyeDisplacement[1]) / 2);
            }

            blink.Push(modelInput.Blink);
            smile.Push(modelInput.Smile);
            frown.Push(modelInput.Frown);
            squint.Push(modelInput.Squint);
            gazeIn.Push(modelInput.GazeIn);
            gazeOut.Push(modelInput.GazeOut);
            headSpeed.Push(modelInput.HeadSpeed);
            eyeDwell.Push(modelInput.EyeDwell);
            headTilt.Push(modelInput.HeadTilt);
            //pressCount.Push();
            //responseTime.Push();

            Console.WriteLine("\n{0} Blink: {1} Smile: {2} Frown: {3} Squint: {4} Gaze in: {5} Gaze out: {6} Head speed: {7} Eye dwell: {8} Head tilt: {9}",
                f.ReadingTimestamp, blink.Mean, squint.Mean, gazeIn.Mean, gazeOut.Mean, smile.Mean, frown.Mean, headSpeed.Mean, eyeDwell.Mean, headTilt.Mean);

            previousReading = f;

            ////////////////////////////////////////////////////////
            ///  ML HERE ...
            ////////////////////////////////////////////////////////
            ///

            ModelOutput prediction = predictionEngine.Predict(modelInput);


            Console.WriteLine("Model Result: {0], {1}", prediction.Prediction, prediction.Score);

            return new Random().NextDouble();

            //// Simulate engangement calculation for development purposes
            //return (double?)faceAnchorReading.FacialExpressions["SmileLeft"];
        }
    }

    public static class MatrixExtensions
    {
        /// <summary>
        /// Returns the row with number 'row' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            var rowLength = matrix.GetLength(1);
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[row, i];

            return rowVector;
        }

        /// <summary>
        /// Sets the row with number 'row' of this 2D-matrix to the parameter 'rowVector'.
        /// </summary>
        public static void SetRow<T>(this T[,] matrix, int row, T[] rowVector)
        {
            var rowLength = matrix.GetLength(1);

            for (var i = 0; i < rowLength; i++)
                matrix[row, i] = rowVector[i];
        }

        /// <summary>
        /// Returns the column with number 'col' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetCol<T>(this T[,] matrix, int col)
        {
            var colLength = matrix.GetLength(0);
            var colVector = new T[colLength];

            for (var i = 0; i < colLength; i++)
                colVector[i] = matrix[i, col];

            return colVector;
        }

        /// <summary>
        /// Sets the column with number 'col' of this 2D-matrix to the parameter 'colVector'.
        /// </summary>
        public static void SetCol<T>(this T[,] matrix, int col, T[] colVector)
        {
            var colLength = matrix.GetLength(0);

            for (var i = 0; i < colLength; i++)
                matrix[i, col] = colVector[i];
        }
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