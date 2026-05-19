using System.Collections.Generic;
using UnityEngine;
using Virtava.Client.Abstractions;

namespace Virtava.Adapters.Unity
{
    public class UnityBlendshapeAnimatable : MonoBehaviour, IBlendshapeAnimatable
    {
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;

        private IEnumerable<string> _availableBlendshapes;

        public IEnumerable<string> AvailableBlendshapes => _availableBlendshapes;

        void Awake()
        {
            var availableBlendshapes = new string[_meshRenderer.sharedMesh.blendShapeCount];
            for (int i = 0; i < availableBlendshapes.Length; i++)
                availableBlendshapes[i] = _meshRenderer.sharedMesh.GetBlendShapeName(i);
            _availableBlendshapes = availableBlendshapes;
        }

        public void ApplyBlendshape(string blendshapeName, float blendshapeValue)
        {
            var index = _meshRenderer.sharedMesh.GetBlendShapeIndex(blendshapeName);
            _meshRenderer.SetBlendShapeWeight(index, blendshapeValue * 100f);
        }
    }
}