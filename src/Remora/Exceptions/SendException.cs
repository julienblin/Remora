using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Exceptions
{
    [Serializable]
    public class SendException : RemoraException
    {
        public SendException()
        {
        }

        public SendException(string message)
            : base(message)
        {
        }

        public SendException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SendException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
