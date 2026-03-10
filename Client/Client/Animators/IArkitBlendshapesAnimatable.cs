namespace Univertracker.Client
{
    public interface IArkitBlendshapesAnimatable
    {
        public void Apply(ArkitBlendshape blendshape, float value, bool omitIfMissing = true);
    }
}