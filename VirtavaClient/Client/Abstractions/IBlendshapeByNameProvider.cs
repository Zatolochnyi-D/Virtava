namespace Virtava.Client.Abstractions
{
    public interface IBlendshapeByNameProvider
    {
        public bool HasBlendshapeByName(string name);
        public float GetBlendshapeByName(string name);
    }
}