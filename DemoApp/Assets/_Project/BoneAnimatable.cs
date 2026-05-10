using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
// using Univertracker.Client;

public class BoneAnimatable : MonoBehaviour//, IArkitBlendshapesAnimatable
{
    [SerializeField] private Transform _rigRoot;
    [SerializeField] private TextAsset _differenceJson;
    [SerializeField] private TextAsset _namingConvetionsMapJson;

    // private ArkitBledshapeDifference _difference;

    void Awake()
    {
        var namingConventionsMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(_namingConvetionsMapJson.text);
        // _difference = ArkitBledshapeDifference.ReadFromJson(_differenceJson.text, namingConventionsMap);
    }

    // public void Apply(ArkitBlendshape blendshape, float value, bool omitIfMissing = true)
    // {
    //     if (omitIfMissing)
    //     {
    //         if (_difference.Differences.TryGetValue(blendshape, out var result))
    //         {
    //             foreach (var difference in result)
    //             {
    //                 Debug.Log(difference.boneName);
    //                 var bone = _rigRoot.RecursiveFind(difference.boneName);
                    
    //                 Debug.Log(bone);
    //                 var startPos = bone.localPosition;
    //                 var startRot = bone.localEulerAngles;
    //                 var startScl = bone.localScale;
    //                 var endPos = startPos + difference.positionDifference.ToVector3();
    //                 var endRot = startRot + difference.rotationDifference.ToVector3();
    //                 var endScl = startScl + difference.scaleDifference.ToVector3();

    //                 bone.localPosition = Vector3.Lerp(startPos, endPos, value);
    //                 bone.localEulerAngles = Vector3.Lerp(startRot, endRot, value);
    //                 bone.localScale = Vector3.Lerp(startScl, endScl, value);
    //             }
    //         }
    //     }
    //     else
    //     {

    //     }
    // }
}

public static class TransformExtension
{
    public static Transform RecursiveFind(this Transform transform, string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
                return child;
            var recursiveChild = child.RecursiveFind(name);
            if (recursiveChild != null)
                return recursiveChild;
        }
        return null;
    }
}

public static class VectorExtension
{
    // public static Vector3 ToVector3(this Vector vector)
    // {
    //     // TODO: define what coordinate system is used in bone map format.
    //     return new Vector3(vector.x, vector.y, vector.z);
    // }
}