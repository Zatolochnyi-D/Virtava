using UnityEngine;

public class Animatable : MonoBehaviour, IBlendshapeAnimatable
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;

    public void ApplyBlendshape(string blendshapeName, float blendshapeValue)
    {
        var index = _meshRenderer.sharedMesh.GetBlendShapeIndex(blendshapeName);
        _meshRenderer.SetBlendShapeWeight(index, blendshapeValue);
    }
}