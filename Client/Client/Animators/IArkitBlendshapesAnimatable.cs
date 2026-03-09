using System.Collections.Generic;

namespace Univertracker.Client
{
    public interface IArkitBlendshapesAnimatable
    {
        public IEnumerable<ArkitBlendshape> ExcludedFromAnimation { get; }

        public void Apply(ArkitBlendshape blendshape, float value, bool omitIfMissing = true);
    }
}