using UnityEngine;
using Virtava.Client;

public class LogicComponent : MonoBehaviour
{
    [SerializeField] private TrackingServerListenerWrapper _wrapper;
    [SerializeField] private UnityBlendshapeAnimatable _animatable;

    private BlendshapeAnimator _animator;

    void Awake()
    {
        _wrapper.OnResultReceived += HandleResults;
        _animator = new(_animatable);
    }

    private void HandleResults(TrackingResult result)
    {
        if (!result.TrackingSucceded)
            return;
        _animator.Animate(new ArkitBlendshapeByNameProvider(result));
        Debug.Log(result.TrackingSucceded);
    }
}

public static class BlendshapesExtension
{
    public static float GetBlendshapeByName(this Blendshapes blendshapes, string name)
    {
        return name switch
        {
            "browDownLeft" => blendshapes.BrowDownLeft,
            "browDownRight" => blendshapes.BrowDownRight,
            "browInnerUp" => blendshapes.BrowInnerUp,
            "browOuterUpLeft" => blendshapes.BrowOuterUpLeft,
            "browOuterUpRight" => blendshapes.BrowOuterUpRight,
            "cheekPuff" => blendshapes.CheekPuff,
            "cheekSquintLeft" => blendshapes.CheekSquintLeft,
            "cheekSquintRight" => blendshapes.CheekSquintRight,
            "eyeBlinkLeft" => blendshapes.EyeBlinkLeft,
            "eyeBlinkRight" => blendshapes.EyeBlinkRight,
            "eyeLookDownLeft" => blendshapes.EyeLookDownLeft,
            "eyeLookDownRight" => blendshapes.EyeLookDownRight,
            "eyeLookInLeft" => blendshapes.EyeLookInLeft,
            "eyeLookInRight" => blendshapes.EyeLookInRight,
            "eyeLookOutLeft" => blendshapes.EyeLookOutLeft,
            "eyeLookOutRight" => blendshapes.EyeLookOutRight,
            "eyeLookUpLeft" => blendshapes.EyeLookUpLeft,
            "eyeLookUpRight" => blendshapes.EyeLookUpRight,
            "eyeSquintLeft" => blendshapes.EyeSquintLeft,
            "eyeSquintRight" => blendshapes.EyeSquintRight,
            "eyeWideLeft" => blendshapes.EyeWideLeft,
            "eyeWideRight" => blendshapes.EyeWideRight,
            "jawForward" => blendshapes.JawForward,
            "jawLeft" => blendshapes.JawLeft,
            "jawOpen" => blendshapes.JawOpen,
            "jawRight" => blendshapes.JawRight,
            "mouthClose" => blendshapes.MouthClose,
            "mouthDimpleLeft" => blendshapes.MouthDimpleLeft,
            "mouthDimpleRight" => blendshapes.MouthDimpleRight,
            "mouthFrownLeft" => blendshapes.MouthFrownLeft,
            "mouthFrownRight" => blendshapes.MouthFrownRight,
            "mouthFunnel" => blendshapes.MouthFunnel,
            "mouthLeft" => blendshapes.MouthLeft,
            "mouthLowerDownLeft" => blendshapes.MouthLowerDownLeft,
            "mouthLowerDownRight" => blendshapes.MouthLowerDownRight,
            "mouthPressLeft" => blendshapes.MouthPressLeft,
            "mouthPressRight" => blendshapes.MouthPressRight,
            "mouthPucker" => blendshapes.MouthPucker,
            "mouthRight" => blendshapes.MouthRight,
            "mouthRollLower" => blendshapes.MouthRollLower,
            "mouthRollUpper" => blendshapes.MouthRollUpper,
            "mouthShrugLower" => blendshapes.MouthShrugLower,
            "mouthShrugUpper" => blendshapes.MouthShrugUpper,
            "mouthSmileLeft" => blendshapes.MouthSmileLeft,
            "mouthSmileRight" => blendshapes.MouthSmileRight,
            "mouthStretchLeft" => blendshapes.MouthStretchLeft,
            "mouthStretchRight" => blendshapes.MouthStretchRight,
            "mouthUpperUpLeft" => blendshapes.MouthUpperUpLeft,
            "mouthUpperUpRight" => blendshapes.MouthUpperUpRight,
            "noseSneerLeft" => blendshapes.NoseSneerLeft,
            "noseSneerRight" => blendshapes.NoseSneerRight,
            "tongueOut" => blendshapes.TongueOut,
            _ => throw new System.ArgumentException($"ARKit Blendshapes do not contain name \"{name}\""),
        };
    }
}