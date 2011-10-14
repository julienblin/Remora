using System;
using System.Runtime.Serialization;

namespace Remora
{
    public class RemoraException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RemoraException() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RemoraException(string message)
            : base(message)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RemoraException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected RemoraException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
