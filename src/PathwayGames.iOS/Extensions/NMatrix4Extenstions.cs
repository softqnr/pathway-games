using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using OpenTK;
using SceneKit;
using UIKit;

namespace PathwayGames.iOS.Extensions
{
    public static class NMatrix4Extenstions
    {
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

        public static float[,] ToFloatMatrix4(this NMatrix4 self)
        {
            return new float[4,4] {
              {self.M11, self.M21, self.M31, self.M41},
              {self.M12, self.M22, self.M32, self.M42},
              {self.M13, self.M23, self.M33, self.M43},
              {self.M14, self.M24, self.M34, self.M44},
            };
        }
    }
}