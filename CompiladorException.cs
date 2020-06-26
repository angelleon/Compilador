namespace Semantica
{
    [System.Serializable]
    public class CompiladorException : System.Exception
    {
        public CompiladorException() { }
        public CompiladorException(string message) :
            base("===========================================================================\n\n" +
                 "Error:\n" +
                 message + "\n\n" +
                 "===========================================================================")
        { }
        public CompiladorException(string message, System.Exception inner) : base(message, inner) { }
        protected CompiladorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}