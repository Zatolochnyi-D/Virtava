using Virtava.Client.Abstractions;

namespace Virtava.Client
{
    public class BlendshapeAnimator
    {
        private IBlendshapeAnimatable _blendshapeAnimatable;

        public BlendshapeAnimator(IBlendshapeAnimatable blendshapeAnimatable)
        {
            _blendshapeAnimatable = blendshapeAnimatable;
        }

        public void Animate<T>(T provider) where T : IBlendshapeByNameProvider
        {
            foreach (var name in _blendshapeAnimatable.AvailableBlendshapes)
            {
                if (provider.HasBlendshapeByName(name))
                    _blendshapeAnimatable.ApplyBlendshape(name, provider.GetBlendshapeByName(name));
                else
                    throw new MissingBlendshapeException($"Couldn't animate object properly - \"{name}\" blendshape is missing.");
            }
        }

        public bool TryAnimate<T>(T provider) where T : IBlendshapeByNameProvider
        {
            var completeSuccess = true;
            foreach (var name in _blendshapeAnimatable.AvailableBlendshapes)
            {
                if (provider.HasBlendshapeByName(name))
                    _blendshapeAnimatable.ApplyBlendshape(name, provider.GetBlendshapeByName(name));
                else
                    completeSuccess = false;
            }
            return completeSuccess;
        }
    }
}