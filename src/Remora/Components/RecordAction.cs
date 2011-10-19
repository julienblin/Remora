using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Components
{
    [DataContract(Name = "action", Namespace = RecordAction.Namespace)]
    public class RecordAction
    {
        public const string Namespace = @"http://remora.org/actions/SOAP";

        [DataMember(Name = "headers")]
        public RecordActionHeader[] Headers { get; set;}

        [DataMember(Name = "requestContentEncoding")]
        public string RequestContentEncoding { get; set; }

        [DataMember(Name = "request")]
        public string Request { get; set; }

        [DataMember(Name = "responseContentEncoding")]
        public string ResponseContentEncoding { get; set; }

        [DataMember(Name = "response")]
        public string Response { get; set; }

        [DataMember(Name = "responseStatusCode")]
        public int ResponseStatusCode { get; set; }

        [DataMember(Name = "onError")]
        public bool OnError { get; set; }
    }

    [DataContract(Name = "header", Namespace = RecordAction.Namespace)]
    public class RecordActionHeader
    {
        public RecordActionHeader()
        {
            
        }

        public RecordActionHeader(string name, string value)
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
