using System;
using System.Linq;

namespace Univertracker.Client
{
    public class ArkitBlendshapesAnimator
    {
        private IArkitBlendshapesAnimatable _animatable;

        public ArkitBlendshapesAnimator(IArkitBlendshapesAnimatable arkitBlendshapesAnimatable)
        {
            _animatable = arkitBlendshapesAnimatable;
        }

        // TODO: define behaviour when some of blendshapes are missing.
        public void Apply(TrackingResult result)
        {
            foreach (var blendshape in Enum.GetNames(typeof(ArkitBlendshape)).Select(x => Enum.Parse<ArkitBlendshape>(x)))
            {
                _animatable.Apply(blendshape, result.GetBlendshape(blendshape));
            }
        }
    }
}