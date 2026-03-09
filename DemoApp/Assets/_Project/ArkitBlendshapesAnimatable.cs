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
    private IEnumerable<ArkitBlendshape> _excludeFromAnimation;

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

        // _excludeFromAnimation = ArkitBlendshapes.BlendshapesList.Where(x => !_blendshapeIndexMap.TryGetValue(x, out _));
        // foreach (var blendshape in _excludeFromAnimation)
        // {
        //     Debug.LogWarning($"{blendshape} was not present on model.");
        // }
    }

    public void Apply(ArkitBlendshape blendshape, float value)
    {
        // if (_excludeFromAnimation.Contains(blendshape))
        //     return;
        _renderer.SetBlendShapeWeight(_blendshapeIndexMap[blendshape], value * 100f);
    }
}