using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArkitBlendshapesAnimatable : MonoBehaviour, IArkitBlendshapesAnimatable
{
    [SerializeField] private SkinnedMeshRenderer _renderer;

    private Dictionary<ArkitBlendshape, int> _blendshapeIndexMap;
    private IEnumerable<ArkitBlendshape> _excludeFromAnimation;

    void Awake()
    {
        _blendshapeIndexMap = new();

        // TODO: move string to enum conversion to Client lib.
        var nameBlendshapeMap = Enum.GetNames(typeof(ArkitBlendshape)).Select(x => (Enum.Parse<ArkitBlendshape>(x), x.ToLower())).ToDictionary(x => x.Item2, x => x.Item1);

        var mesh = _renderer.sharedMesh;
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            var name = mesh.GetBlendShapeName(i).ToLower();
            var blendshape = nameBlendshapeMap[name];
            _blendshapeIndexMap[blendshape] = i;
        }

        _excludeFromAnimation = nameBlendshapeMap.Values.Where(x => !_blendshapeIndexMap.TryGetValue(x, out _));
        foreach (var blendshape in _excludeFromAnimation)
        {
            Debug.LogWarning($"{blendshape} was not present on model.");
        }
    }

    public void Apply(ArkitBlendshape blendshape, float value)
    {
        Debug.Log(blendshape);
        Debug.Log(_excludeFromAnimation);
        if (_excludeFromAnimation.Contains(blendshape))
            return;
        _renderer.SetBlendShapeWeight(_blendshapeIndexMap[blendshape], value * 100f);
    }
}