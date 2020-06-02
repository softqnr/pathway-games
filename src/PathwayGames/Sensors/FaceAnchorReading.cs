using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PathwayGames.Sensors
{
    public class FaceAnchorReading
    {
        public DateTime ReadingDate { get; set; }

        public double ReadingTimestamp { get; set; }

        //public double ReadingTimespan { get; set; }

        public float[,] FaceTransform { get; set; }

        public float[,] LeftEyeTransform { get; set; }

        public float[,] RightEyeTransform { get; set; }

        public float[] LookAtPointTransform { get; set; }

        public Dictionary<string, float?> FacialExpressions = new Dictionary<string, float?>();

        public FaceAnchorReading( double readingTimestamp, float[,] faceTransform,
            float[,] leftEyeTransform, float[,] rightEyeTransform, float[] lookAtPointTransform, Dictionary<string, float?>  facialExpressions)
        {
            ReadingDate = DateTime.Now;
            //ReadingTimespan = readingTimespan;
            ReadingTimestamp = readingTimestamp;
            FaceTransform = faceTransform;
            LeftEyeTransform = leftEyeTransform;
            RightEyeTransform = rightEyeTransform;
            LookAtPointTransform = lookAtPointTransform;
            FacialExpressions = facialExpressions;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
