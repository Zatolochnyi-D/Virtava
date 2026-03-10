using System;
using System.Collections.Generic;
using System.Linq;

namespace Univertracker.Client
{
    public enum ArkitBlendshape
    {
        BrowDownLeft = 1,
        BrowDownRight = 2,
        BrowInnerUp = 3,
        BrowOuterUpLeft = 4,
        BrowOuterUpRight = 5,
        CheekPuff = 6,
        CheekSquintLeft = 7,
        CheekSquintRight = 8,
        EyeBlinkLeft = 9,
        EyeBlinkRight = 10,
        EyeLookDownLeft = 11,
        EyeLookDownRight = 12,
        EyeLookInLeft = 13,
        EyeLookInRight = 14,
        EyeLookOutLeft = 15,
        EyeLookOutRight = 16,
        EyeLookUpLeft = 17,
        EyeLookUpRight = 18,
        EyeSquintLeft = 19,
        EyeSquintRight = 20,
        EyeWideLeft = 21,
        EyeWideRight = 22,
        JawForward = 23,
        JawLeft = 24,
        JawOpen = 25,
        JawRight = 26,
        MouthClose = 27,
        MouthDimpleLeft = 28,
        MouthDimpleRight = 29,
        MouthFrownLeft = 30,
        MouthFrownRight = 31,
        MouthFunnel = 32,
        MouthLeft = 33,
        MouthLowerDownLeft = 34,
        MouthLowerDownRight = 35,
        MouthPressLeft = 36,
        MouthPressRight = 37,
        MouthPucker = 38,
        MouthRight = 39,
        MouthRollLower = 40,
        MouthRollUpper = 41,
        MouthShrugLower = 42,
        MouthShrugUpper = 43,
        MouthSmileLeft = 44,
        MouthSmileRight = 45,
        MouthStretchLeft = 46,
        MouthStretchRight = 47,
        MouthUpperUpLeft = 48,
        MouthUpperUpRight = 49,
        NoseSneerLeft = 50,
        NoseSneerRight = 51,
        TongueOut = 52,
    }

    public static class ArkitBlendshapes
    {
        // TODO: we can initialize that list using naming convention map (enum value names (PascalCase) convert to camelCase).
        private static readonly Dictionary<string, ArkitBlendshape> _stringNameToBlendshapeMap = new Dictionary<string, ArkitBlendshape>()
        {
            ["browDownLeft"] = ArkitBlendshape.BrowDownLeft,
            ["browDownRight"] = ArkitBlendshape.BrowDownRight,
            ["browInnerUp"] = ArkitBlendshape.BrowInnerUp,
            ["browOuterUpLeft"] = ArkitBlendshape.BrowOuterUpLeft,
            ["browOuterUpRight"] = ArkitBlendshape.BrowOuterUpRight,
            ["cheekPuff"] = ArkitBlendshape.CheekPuff,
            ["cheekSquintLeft"] = ArkitBlendshape.CheekSquintLeft,
            ["cheekSquintRight"] = ArkitBlendshape.CheekSquintRight,
            ["eyeBlinkLeft"] = ArkitBlendshape.EyeBlinkLeft,
            ["eyeBlinkRight"] = ArkitBlendshape.EyeBlinkRight,
            ["eyeLookDownLeft"] = ArkitBlendshape.EyeLookDownLeft,
            ["eyeLookDownRight"] = ArkitBlendshape.EyeLookDownRight,
            ["eyeLookInLeft"] = ArkitBlendshape.EyeLookInLeft,
            ["eyeLookInRight"] = ArkitBlendshape.EyeLookInRight,
            ["eyeLookOutLeft"] = ArkitBlendshape.EyeLookOutLeft,
            ["eyeLookOutRight"] = ArkitBlendshape.EyeLookOutRight,
            ["eyeLookUpLeft"] = ArkitBlendshape.EyeLookUpLeft,
            ["eyeLookUpRight"] = ArkitBlendshape.EyeLookUpRight,
            ["eyeSquintLeft"] = ArkitBlendshape.EyeSquintLeft,
            ["eyeSquintRight"] = ArkitBlendshape.EyeSquintRight,
            ["eyeWideLeft"] = ArkitBlendshape.EyeWideLeft,
            ["eyeWideRight"] = ArkitBlendshape.EyeWideRight,
            ["jawForward"] = ArkitBlendshape.JawForward,
            ["jawLeft"] = ArkitBlendshape.JawLeft,
            ["jawOpen"] = ArkitBlendshape.JawOpen,
            ["jawRight"] = ArkitBlendshape.JawRight,
            ["mouthClose"] = ArkitBlendshape.MouthClose,
            ["mouthDimpleLeft"] = ArkitBlendshape.MouthDimpleLeft,
            ["mouthDimpleRight"] = ArkitBlendshape.MouthDimpleRight,
            ["mouthFrownLeft"] = ArkitBlendshape.MouthFrownLeft,
            ["mouthFrownRight"] = ArkitBlendshape.MouthFrownRight,
            ["mouthFunnel"] = ArkitBlendshape.MouthFunnel,
            ["mouthLeft"] = ArkitBlendshape.MouthLeft,
            ["mouthLowerDownLeft"] = ArkitBlendshape.MouthLowerDownLeft,
            ["mouthLowerDownRight"] = ArkitBlendshape.MouthLowerDownRight,
            ["mouthPressLeft"] = ArkitBlendshape.MouthPressLeft,
            ["mouthPressRight"] = ArkitBlendshape.MouthPressRight,
            ["mouthPucker"] = ArkitBlendshape.MouthPucker,
            ["mouthRight"] = ArkitBlendshape.MouthRight,
            ["mouthRollLower"] = ArkitBlendshape.MouthRollLower,
            ["mouthRollUpper"] = ArkitBlendshape.MouthRollUpper,
            ["mouthShrugLower"] = ArkitBlendshape.MouthShrugLower,
            ["mouthShrugUpper"] = ArkitBlendshape.MouthShrugUpper,
            ["mouthSmileLeft"] = ArkitBlendshape.MouthSmileLeft,
            ["mouthSmileRight"] = ArkitBlendshape.MouthSmileRight,
            ["mouthStretchLeft"] = ArkitBlendshape.MouthStretchLeft,
            ["mouthStretchRight"] = ArkitBlendshape.MouthStretchRight,
            ["mouthUpperUpLeft"] = ArkitBlendshape.MouthUpperUpLeft,
            ["mouthUpperUpRight"] = ArkitBlendshape.MouthUpperUpRight,
            ["noseSneerLeft"] = ArkitBlendshape.NoseSneerLeft,
            ["noseSneerRight"] = ArkitBlendshape.NoseSneerRight,
            ["tongueOut"] = ArkitBlendshape.TongueOut,
        };
        private static readonly Dictionary<ArkitBlendshape, string> _blendshapeToStringNameMap = new Dictionary<ArkitBlendshape, string>();
        public static readonly IEnumerable<ArkitBlendshape> BlendshapesList = Enum.GetNames(typeof(ArkitBlendshape)).Select(x => Enum.Parse<ArkitBlendshape>(x));

        static ArkitBlendshapes()
        {
            foreach (var (name, blendshape) in _stringNameToBlendshapeMap)
                _blendshapeToStringNameMap[blendshape] = name;
        }

        public static ArkitBlendshape GetBlendshape(string blendshapeName, Dictionary<string, string> namingConventionsMap)
        {
            // TODO: handle errors on name missing from convention map.
            return _stringNameToBlendshapeMap[namingConventionsMap[blendshapeName]];
            // _ => throw new ArgumentException($"Provided name \"{blendshapeName}\" doesn't exist, written wrong, or used naming conversion is not supported."),
        }
    }
}