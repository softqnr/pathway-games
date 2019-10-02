using ARKit;
using System.Collections.Generic;

namespace PathwayGames.iOS.Extensions
{
    public static class ARBlendShapeLocationOptionsExtensions
    {
        public static Dictionary<string, float?> ToDictionary(this ARBlendShapeLocationOptions self)
        {
            return new Dictionary<string, float?>()
                    {
                        { "BrowDownLeft", self.BrowDownLeft },
                        { "BrowDownRight", self.BrowDownRight },
                        { "BrowInnerUp", self.BrowInnerUp },
                        { "BrowOuterUpLeft", self.BrowOuterUpLeft },
                        { "BrowOuterUpRight", self.BrowOuterUpRight },
                        { "CheekPuff", self.CheekPuff },
                        { "CheekSquintLeft", self.CheekSquintLeft },
                        { "CheekSquintRight", self.CheekSquintRight },
                        { "EyeBlinkLeft", self.EyeBlinkLeft },
                        { "EyeBlinkRight", self.EyeBlinkRight },
                        { "EyeLookDownLeft", self.EyeLookDownLeft },
                        { "EyeLookDownRight", self.EyeLookDownRight },
                        { "EyeLookInLeft", self.EyeLookInLeft },
                        { "EyeLookInRight", self.EyeLookInRight },
                        { "EyeLookOutLeft", self.EyeLookOutLeft },
                        { "EyeLookOutRight", self.EyeLookOutRight },
                        { "EyeLookUpLeft", self.EyeLookUpLeft },
                        { "EyeLookUpRight", self.EyeLookUpRight },
                        { "EyeSquintLeft", self.EyeSquintLeft },
                        { "EyeSquintRight", self.EyeSquintRight },
                        { "EyeWideLeft", self.EyeWideLeft },
                        { "EyeWideRight", self.EyeWideRight },
                        { "JawForward", self.JawForward },
                        { "JawLeft", self.JawLeft },
                        { "JawOpen", self.JawOpen },
                        { "JawRight", self.JawRight },
                        { "MouthClose", self.MouthClose },
                        { "MouthDimpleLeft", self.MouthDimpleLeft },
                        { "MouthDimpleRight", self.MouthDimpleRight },
                        { "MouthFrownLeft", self.MouthFrownLeft },
                        { "MouthFrownRight", self.MouthFrownRight },
                        { "MouthFunnel", self.MouthFunnel },
                        { "MouthLeft", self.MouthLeft },
                        { "MouthLowerDownLeft", self.MouthLowerDownLeft },
                        { "MouthLowerDownRight", self.MouthLowerDownRight },
                        { "MouthPressLeft", self.MouthPressLeft },
                        { "MouthPressRight", self.MouthPressRight },
                        { "MouthPucker", self.MouthPucker },
                        { "MouthRight", self.MouthRight },
                        { "MouthRollLower", self.MouthRollLower },
                        { "MouthRollUpper", self.MouthRollUpper },
                        { "MouthShrugLower", self.MouthShrugLower },
                        { "MouthShrugUpper", self.MouthShrugUpper },
                        { "MouthSmileLeft", self.MouthSmileLeft },
                        { "MouthSmileRight", self.MouthSmileRight },
                        { "MouthStretchLeft", self.MouthStretchLeft },
                        { "MouthStretchRight", self.MouthStretchRight },
                        { "MouthUpperUpLeft", self.MouthUpperUpLeft },
                        { "MouthUpperUpRight", self.MouthUpperUpRight },
                        { "NoseSneerLeft", self.NoseSneerLeft },
                        { "NoseSneerRight", self.NoseSneerRight },
                        { "TongueOut", self.TongueOut }
                    };
        }
    }
}