using System;
using Virtava.Client.Abstractions;

namespace Virtava.DataFormatModules.ArkitBlendshapes
{
    public readonly struct ArkitBlendshapeByNameProvider : IBlendshapeByNameProvider
    {
        private readonly ArkitBlendshapesResult _trackingResult;

        public ArkitBlendshapeByNameProvider(ArkitBlendshapesResult trackingResult)
        {
            _trackingResult = trackingResult;
        }

        public bool HasBlendshapeByName(string name)
        {
            try
            {
                _trackingResult.Blendshapes.GetBlendshapeByName(name);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        public float GetBlendshapeByName(string name)
        {
            return _trackingResult.Blendshapes.GetBlendshapeByName(name);
        }
    }
}