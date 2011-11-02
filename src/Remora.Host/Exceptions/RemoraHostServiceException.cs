using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Host.Exceptions
{
    [Serializable]
    public class RemoraHostServiceException : RemoraHostException
    {
        public RemoraHostServiceException()
        {
        }

        public RemoraHostServiceException(string message)
            : base(message)
        {
        }

        public RemoraHostServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RemoraHostServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
