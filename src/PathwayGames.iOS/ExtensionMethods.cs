using System;
using Xamarin.Forms.Platform.iOS;

using ARKit;
using SceneKit;
using OpenTK;
using System.Collections.Generic;

namespace PathwayGames.iOS.Sensors
{
    public static class ExtensionMethods
    {
        public static double Length(this SCNVector3 v)
        {
            return Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }
        //public static SCNVector3 operator- (SCNVector3 l, SCNVector3 r)
        //{
        //    return SCNVector3Make(l.X - r.X, l.Y - r.Y, l.Z - r.Z); 
        //}

        public static float Average(this IEnumerable<nfloat> positions)
        {
            float result = 0;
            float sum = 0;
            float count = 0;
            foreach (float e in positions)
            {
                sum += e;
                count++;
            }
            result = (count > 0) ? sum / count : 0;
            return result;
        }

        public static SCNMatrix4 ToSCNMatrix4(this NMatrix4 self)
        {
            var newMatrix = new SCNMatrix4(
                self.M11, self.M21, self.M31, self.M41,
                self.M12, self.M22, self.M32, self.M42,
                self.M13, self.M23, self.M33, self.M43,
                self.M14, self.M24, self.M34, self.M44
            );

            return newMatrix;
        }
    }
}