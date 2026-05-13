namespace Virtava.Client
{
    [System.Serializable]
    public class MissingBlendshapeException : System.Exception
    {
        public MissingBlendshapeException() { }
        public MissingBlendshapeException(string message) : base(message) { }
        public MissingBlendshapeException(string message, System.Exception inner) : base(message, inner) { }
        protected MissingBlendshapeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}