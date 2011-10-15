using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Exceptions
{
    [Serializable]
    public class SoapParsingException : RemoraException
    {
        public SoapParsingException()
        {
        }

        public SoapParsingException(string message)
            : base(message)
        {
        }

        public SoapParsingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SoapParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
