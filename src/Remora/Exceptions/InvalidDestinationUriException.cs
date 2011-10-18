using System;
using System.Runtime.Serialization;

namespace Remora.Exceptions
{
    [Serializable]
    public class InvalidDestinationUriException : RemoraException
    {
        public InvalidDestinationUriException()
        {
        }

        public InvalidDestinationUriException(string message)
            : base(message)
        {
        }

        public InvalidDestinationUriException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidDestinationUriException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
