using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Univertracker.Client
{
    public enum ArkitBlendshape
    {
        BrowDownLeft,
        BrowDownRight,
        BrowInnerUp,
        BrowOuterUpLeft,
        BrowOuterUpRight,
        CheekPuff,
        CheekSquintLeft,
        CheekSquintRight,
        EyeBlinkLeft,
        EyeBlinkRight,
        EyeLookDownLeft,
        EyeLookDownRight,
        EyeLookInLeft,
        EyeLookInRight,
        EyeLookOutLeft,
        EyeLookOutRight,
        EyeLookUpLeft,
        EyeLookUpRight,
        EyeSquintLeft,
        EyeSquintRight,
        EyeWideLeft,
        EyeWideRight,
        JawForward,
        JawLeft,
        JawOpen,
        JawRight,
        MouthClose,
        MouthDimpleLeft,
        MouthDimpleRight,
        MouthFrownLeft,
        MouthFrownRight,
        MouthFunnel,
        MouthLeft,
        MouthLowerDownLeft,
        MouthLowerDownRight,
        MouthPressLeft,
        MouthPressRight,
        MouthPucker,
        MouthRight,
        MouthRollLower,
        MouthRollUpper,
        MouthShrugLower,
        MouthShrugUpper,
        MouthSmileLeft,
        MouthSmileRight,
        MouthStretchLeft,
        MouthStretchRight,
        MouthUpperUpLeft,
        MouthUpperUpRight,
        NoseSneerLeft,
        NoseSneerRight,
        TongueOut,
    }

    public static class ArkitBlendshapes
    {
        private static readonly Dictionary<string, string> _blendshapeNamesMap;
        public static readonly IEnumerable<ArkitBlendshape> BlendshapesList = Enum.GetNames(typeof(ArkitBlendshape)).Select(x => Enum.Parse<ArkitBlendshape>(x));

        static ArkitBlendshapes()
        {
            _blendshapeNamesMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("blendshapeNamesMap.json"))!; // TODO: find a better way to load that file.
        }

        public static ArkitBlendshape GetBlendshape(string blendshapeName)
        {
            return _blendshapeNamesMap[blendshapeName] switch
            {
                "browDownLeft" => ArkitBlendshape.BrowDownLeft,
                "browDownRight" => ArkitBlendshape.BrowDownRight,
                "browInnerUp" => ArkitBlendshape.BrowInnerUp,
                "browOuterUpLeft" => ArkitBlendshape.BrowOuterUpLeft,
                "browOuterUpRight" => ArkitBlendshape.BrowOuterUpRight,
                "cheekPuff" => ArkitBlendshape.CheekPuff,
                "cheekSquintLeft" => ArkitBlendshape.CheekSquintLeft,
                "cheekSquintRight" => ArkitBlendshape.CheekSquintRight,
                "eyeBlinkLeft" => ArkitBlendshape.EyeBlinkLeft,
                "eyeBlinkRight" => ArkitBlendshape.EyeBlinkRight,
                "eyeLookDownLeft" => ArkitBlendshape.EyeLookDownLeft,
                "eyeLookDownRight" => ArkitBlendshape.EyeLookDownRight,
                "eyeLookInLeft" => ArkitBlendshape.EyeLookInLeft,
                "eyeLookInRight" => ArkitBlendshape.EyeLookInRight,
                "eyeLookOutLeft" => ArkitBlendshape.EyeLookOutLeft,
                "eyeLookOutRight" => ArkitBlendshape.EyeLookOutRight,
                "eyeLookUpLeft" => ArkitBlendshape.EyeLookUpLeft,
                "eyeLookUpRight" => ArkitBlendshape.EyeLookUpRight,
                "eyeSquintLeft" => ArkitBlendshape.EyeSquintLeft,
                "eyeSquintRight" => ArkitBlendshape.EyeSquintRight,
                "eyeWideLeft" => ArkitBlendshape.EyeWideLeft,
                "eyeWideRight" => ArkitBlendshape.EyeWideRight,
                "jawForward" => ArkitBlendshape.JawForward,
                "jawLeft" => ArkitBlendshape.JawLeft,
                "jawOpen" => ArkitBlendshape.JawOpen,
                "jawRight" => ArkitBlendshape.JawRight,
                "mouthClose" => ArkitBlendshape.MouthClose,
                "mouthDimpleLeft" => ArkitBlendshape.MouthDimpleLeft,
                "mouthDimpleRight" => ArkitBlendshape.MouthDimpleRight,
                "mouthFrownLeft" => ArkitBlendshape.MouthFrownLeft,
                "mouthFrownRight" => ArkitBlendshape.MouthFrownRight,
                "mouthFunnel" => ArkitBlendshape.MouthFunnel,
                "mouthLeft" => ArkitBlendshape.MouthLeft,
                "mouthLowerDownLeft" => ArkitBlendshape.MouthLowerDownLeft,
                "mouthLowerDownRight" => ArkitBlendshape.MouthLowerDownRight,
                "mouthPressLeft" => ArkitBlendshape.MouthPressLeft,
                "mouthPressRight" => ArkitBlendshape.MouthPressRight,
                "mouthPucker" => ArkitBlendshape.MouthPucker,
                "mouthRight" => ArkitBlendshape.MouthRight,
                "mouthRollLower" => ArkitBlendshape.MouthRollLower,
                "mouthRollUpper" => ArkitBlendshape.MouthRollUpper,
                "mouthShrugLower" => ArkitBlendshape.MouthShrugLower,
                "mouthShrugUpper" => ArkitBlendshape.MouthShrugUpper,
                "mouthSmileLeft" => ArkitBlendshape.MouthSmileLeft,
                "mouthSmileRight" => ArkitBlendshape.MouthSmileRight,
                "mouthStretchLeft" => ArkitBlendshape.MouthStretchLeft,
                "mouthStretchRight" => ArkitBlendshape.MouthStretchRight,
                "mouthUpperUpLeft" => ArkitBlendshape.MouthUpperUpLeft,
                "mouthUpperUpRight" => ArkitBlendshape.MouthUpperUpRight,
                "noseSneerLeft" => ArkitBlendshape.NoseSneerLeft,
                "noseSneerRight" => ArkitBlendshape.NoseSneerRight,
                "tongueOut" => ArkitBlendshape.TongueOut,
                _ => throw new ArgumentException($"Provided name \"{blendshapeName}\" doesn't exist, or written wrong, or used naming conversion is not supported."),
            };
        }
    }
}