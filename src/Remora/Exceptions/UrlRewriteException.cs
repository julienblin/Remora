using System;
using System.Runtime.Serialization;

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
