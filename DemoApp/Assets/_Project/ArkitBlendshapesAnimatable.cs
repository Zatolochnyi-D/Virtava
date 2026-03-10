using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Univertracker.Client;

public class ArkitBlendshapesAnimatable : MonoBehaviour, IArkitBlendshapesAnimatable
{
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private TextAsset _namingConvetionsMapJson;

    private Dictionary<string, string> _namingConventionsMap;
    private Dictionary<ArkitBlendshape, int> _blendshapeIndexMap;

    void Awake()
    {
        _namingConventionsMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(_namingConvetionsMapJson.text);
        _blendshapeIndexMap = new();

        var mesh = _renderer.sharedMesh;
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            var name = mesh.GetBlendShapeName(i);
            _blendshapeIndexMap[ArkitBlendshapes.GetBlendshape(name, _namingConventionsMap)] = i;
        }
    }

    public void Apply(ArkitBlendshape blendshape, float value, bool omitIfMissing = true)
    {
        if (omitIfMissing)
        {
            if (_blendshapeIndexMap.TryGetValue(blendshape, out var index))
                _renderer.SetBlendShapeWeight(index, value * 100f);
        }
        else
        {
            _renderer.SetBlendShapeWeight(_blendshapeIndexMap[blendshape], value * 100f);
        }
    }
}