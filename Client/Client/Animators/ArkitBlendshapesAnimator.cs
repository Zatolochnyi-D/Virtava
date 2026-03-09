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

        public void Apply(TrackingResult result, bool omitMissingBlendshapes = true)
        {
            foreach (var blendshape in ArkitBlendshapes.BlendshapesList.Except(_animatable.ExcludedFromAnimation))
            {
                _animatable.Apply(blendshape, result.GetBlendshape(blendshape), omitMissingBlendshapes);
            }
        }
    }
}