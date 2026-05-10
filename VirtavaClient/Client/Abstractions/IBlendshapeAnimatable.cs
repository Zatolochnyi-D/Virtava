using System.Collections.Generic;

namespace Virtava.Client.Abstractions
{
    public interface IBlendshapeAnimatable
    {
        public IEnumerable<string> AvailableBlendshapes { get; }
        public void ApplyBlendshape(string blendshapeName, float blendshapeValue);
    }
}