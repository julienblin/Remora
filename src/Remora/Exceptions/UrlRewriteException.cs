using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Exceptions
{
    [Serializable]
    public class UrlRewriteException : RemoraException
    {
        public UrlRewriteException()
        {
        }

        public UrlRewriteException(string message)
            : base(message)
        {
        }

        public UrlRewriteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UrlRewriteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
