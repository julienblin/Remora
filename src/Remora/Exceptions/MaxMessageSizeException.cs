using System;
using System.Runtime.Serialization;

namespace Remora.Exceptions
{
    [Serializable]
    public class MaxMessageSizeException : RemoraException
    {
        public MaxMessageSizeException()
        {
        }

        public MaxMessageSizeException(string message)
            : base(message)
        {
        }

        public MaxMessageSizeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MaxMessageSizeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
