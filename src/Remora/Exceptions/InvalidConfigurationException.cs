﻿using System;
using System.Runtime.Serialization;

namespace Remora.Exceptions
{
    [Serializable]
    public class InvalidConfigurationException : RemoraException
    {
        public InvalidConfigurationException()
        {
        }

        public InvalidConfigurationException(string message)
            : base(message)
        {
        }

        public InvalidConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
