using OpenTK;

namespace PathwayGames.iOS.Extensions
{
    public static class NVector3Extensions
    {
        public static float[] ToFloatVector3(this NVector3 self)
        {
            return new float[3] { self.X, self.Y, self.Z };
        }
    }
}