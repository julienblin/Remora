using System;
using System.Runtime.Serialization;

namespace Remora.Exceptions
{
    [Serializable]
    public abstract class RemoraException : Exception
    {
        protected RemoraException()
        {
        }

        protected RemoraException(string message)
            : base(message)
        {
        }

        protected RemoraException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RemoraException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
