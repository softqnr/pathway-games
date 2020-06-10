﻿using Newtonsoft.Json;
using PathwayGames.Infrastructure.Json;
using System;
using System.Collections.Generic;

namespace PathwayGames.Sensors
{
    public class FaceAnchorReading
    {
        public DateTime ReadingDate { get; set; }

        [JsonConverter(typeof(UnixTimeMillisecondsConverter))]
        public DateTime ReadingTimestamp { get; set; }

        public float[,] FaceTransform { get; set; }

        public float[,] LeftEyeTransform { get; set; }

        public float[,] RightEyeTransform { get; set; }

        public float[] LookAtPointTransform { get; set; }

        public Dictionary<string, float?> FacialExpressions = new Dictionary<string, float?>();

        public FaceAnchorReading( DateTime readingDate, float[,] faceTransform,
            float[,] leftEyeTransform, float[,] rightEyeTransform, float[] lookAtPointTransform, Dictionary<string, float?>  facialExpressions)
        {
            ReadingDate = readingDate;
            ReadingTimestamp = readingDate;
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
