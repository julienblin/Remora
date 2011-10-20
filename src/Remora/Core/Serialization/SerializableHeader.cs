using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Remora.Core.Impl;

namespace Remora.Core.Serialization
{
    [DataContract(Name = "header", Namespace = SerializationConstants.Namespace)]
    public class SerializableHeader
    {
        public SerializableHeader()
        {
        }

        public SerializableHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
