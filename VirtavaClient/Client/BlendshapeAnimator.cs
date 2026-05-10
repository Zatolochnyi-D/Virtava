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
                {
                    _blendshapeAnimatable.ApplyBlendshape(name, provider.GetBlendshapeByName(name)); // TODO: handle possible different naming formats.
                }
                else
                {
                    // TODO: handle what to do on missing name in provider. Log, throw or ignore?
                }
            }
        }
    }
}