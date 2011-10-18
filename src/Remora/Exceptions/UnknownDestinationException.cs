using System;
using System.Runtime.Serialization;

namespace Remora.Exceptions
{
    [Serializable]
    public class UnknownDestinationException : RemoraException
    {
        public UnknownDestinationException()
        {
        }

        public UnknownDestinationException(string message)
            : base(message)
        {
        }

        public UnknownDestinationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UnknownDestinationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
