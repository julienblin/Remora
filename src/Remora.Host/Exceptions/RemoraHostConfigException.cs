using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Host.Exceptions
{
    [Serializable]
    public class RemoraHostConfigException : RemoraHostException
    {
        public RemoraHostConfigException()
        {
        }

        public RemoraHostConfigException(string message)
            : base(message)
        {
        }

        public RemoraHostConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RemoraHostConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
