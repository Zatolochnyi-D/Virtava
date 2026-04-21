using System;

namespace Virtava.Client
{
    public static class TrackingResultsExtension
    {
        public static float GetBlendshape(this TrackingResult result, ArkitBlendshape blendshape)
        {
            return blendshape switch
            {
                ArkitBlendshape.BrowDownLeft => result.Blendshapes.BrowDownLeft,
                ArkitBlendshape.BrowDownRight => result.Blendshapes.BrowDownRight,
                ArkitBlendshape.BrowInnerUp => result.Blendshapes.BrowInnerUp,
                ArkitBlendshape.BrowOuterUpLeft => result.Blendshapes.BrowOuterUpLeft,
                ArkitBlendshape.BrowOuterUpRight => result.Blendshapes.BrowOuterUpRight,
                ArkitBlendshape.CheekPuff => result.Blendshapes.CheekPuff,
                ArkitBlendshape.CheekSquintLeft => result.Blendshapes.CheekSquintLeft,
                ArkitBlendshape.CheekSquintRight => result.Blendshapes.CheekSquintRight,
                ArkitBlendshape.EyeBlinkLeft => result.Blendshapes.EyeBlinkLeft,
                ArkitBlendshape.EyeBlinkRight => result.Blendshapes.EyeBlinkRight,
                ArkitBlendshape.EyeLookDownLeft => result.Blendshapes.EyeLookDownLeft,
                ArkitBlendshape.EyeLookDownRight => result.Blendshapes.EyeLookDownRight,
                ArkitBlendshape.EyeLookInLeft => result.Blendshapes.EyeLookInLeft,
                ArkitBlendshape.EyeLookInRight => result.Blendshapes.EyeLookInRight,
                ArkitBlendshape.EyeLookOutLeft => result.Blendshapes.EyeLookOutLeft,
                ArkitBlendshape.EyeLookOutRight => result.Blendshapes.EyeLookOutRight,
                ArkitBlendshape.EyeLookUpLeft => result.Blendshapes.EyeLookUpLeft,
                ArkitBlendshape.EyeLookUpRight => result.Blendshapes.EyeLookUpRight,
                ArkitBlendshape.EyeSquintLeft => result.Blendshapes.EyeSquintLeft,
                ArkitBlendshape.EyeSquintRight => result.Blendshapes.EyeSquintRight,
                ArkitBlendshape.EyeWideLeft => result.Blendshapes.EyeWideLeft,
                ArkitBlendshape.EyeWideRight => result.Blendshapes.EyeWideRight,
                ArkitBlendshape.JawForward => result.Blendshapes.JawForward,
                ArkitBlendshape.JawLeft => result.Blendshapes.JawLeft,
                ArkitBlendshape.JawOpen => result.Blendshapes.JawOpen,
                ArkitBlendshape.JawRight => result.Blendshapes.JawRight,
                ArkitBlendshape.MouthClose => result.Blendshapes.MouthClose,
                ArkitBlendshape.MouthDimpleLeft => result.Blendshapes.MouthDimpleLeft,
                ArkitBlendshape.MouthDimpleRight => result.Blendshapes.MouthDimpleRight,
                ArkitBlendshape.MouthFrownLeft => result.Blendshapes.MouthFrownLeft,
                ArkitBlendshape.MouthFrownRight => result.Blendshapes.MouthFrownRight,
                ArkitBlendshape.MouthFunnel => result.Blendshapes.MouthFunnel,
                ArkitBlendshape.MouthLeft => result.Blendshapes.MouthLeft,
                ArkitBlendshape.MouthLowerDownLeft => result.Blendshapes.MouthLowerDownLeft,
                ArkitBlendshape.MouthLowerDownRight => result.Blendshapes.MouthLowerDownRight,
                ArkitBlendshape.MouthPressLeft => result.Blendshapes.MouthPressLeft,
                ArkitBlendshape.MouthPressRight => result.Blendshapes.MouthPressRight,
                ArkitBlendshape.MouthPucker => result.Blendshapes.MouthPucker,
                ArkitBlendshape.MouthRight => result.Blendshapes.MouthRight,
                ArkitBlendshape.MouthRollLower => result.Blendshapes.MouthRollLower,
                ArkitBlendshape.MouthRollUpper => result.Blendshapes.MouthRollUpper,
                ArkitBlendshape.MouthShrugLower => result.Blendshapes.MouthShrugLower,
                ArkitBlendshape.MouthShrugUpper => result.Blendshapes.MouthShrugUpper,
                ArkitBlendshape.MouthSmileLeft => result.Blendshapes.MouthSmileLeft,
                ArkitBlendshape.MouthSmileRight => result.Blendshapes.MouthSmileRight,
                ArkitBlendshape.MouthStretchLeft => result.Blendshapes.MouthStretchLeft,
                ArkitBlendshape.MouthStretchRight => result.Blendshapes.MouthStretchRight,
                ArkitBlendshape.MouthUpperUpLeft => result.Blendshapes.MouthUpperUpLeft,
                ArkitBlendshape.MouthUpperUpRight => result.Blendshapes.MouthUpperUpRight,
                ArkitBlendshape.NoseSneerLeft => result.Blendshapes.NoseSneerLeft,
                ArkitBlendshape.NoseSneerRight => result.Blendshapes.NoseSneerRight,
                ArkitBlendshape.TongueOut => result.Blendshapes.TongueOut,
                _ => throw new ArgumentException($"Non-existent {nameof(ArkitBlendshape)} value provided."),
            };
        }
    }
}