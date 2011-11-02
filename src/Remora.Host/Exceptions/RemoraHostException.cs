using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Host.Exceptions
{
    [Serializable]
    public abstract class RemoraHostException : Exception
    {
        protected RemoraHostException()
        {
        }

        protected RemoraHostException(string message)
            : base(message)
        {
        }

        protected RemoraHostException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RemoraHostException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
