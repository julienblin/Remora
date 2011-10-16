using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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
