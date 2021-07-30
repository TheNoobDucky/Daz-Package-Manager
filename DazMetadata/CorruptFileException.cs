using System;

namespace DazPackage
{
    /// <summary>
    /// Raised when a zip file contents are corrupt.
    /// </summary>
    [Serializable]
    public class CorruptFileException : Exception
    {
        public CorruptFileException() : base() { }
        public CorruptFileException(string message) : base(message) { }
        public CorruptFileException(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected CorruptFileException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
