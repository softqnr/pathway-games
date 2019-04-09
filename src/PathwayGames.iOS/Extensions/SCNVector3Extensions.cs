using SceneKit;
using System;

namespace PathwayGames.iOS.Extensions
{
    public static class SCNVector3Extensions
    {
        public static double Length(this SCNVector3 v)
        {
            return Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        //public static SCNVector3 operator- (SCNVector3 l, SCNVector3 r)
        //{
        //    return SCNVector3Make(l.X - r.X, l.Y - r.Y, l.Z - r.Z); 
        //}
    }
}