using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Remora.Core.Impl;

namespace Remora.Core.Serialization
{
    [DataContract(Name = "request", Namespace = SerializationConstants.Namespace)]
    public class SerializableRequest
    {
        public SerializableRequest()
        {
        }

        public SerializableRequest(IRemoraRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            Contract.EndContractBlock();

            Headers = request.HttpHeaders.Select(k => new SerializableHeader(k.Key, k.Value)).ToArray();
            ContentEncoding = request.ContentEncoding != null ? request.ContentEncoding.HeaderName : null;
            if (request.Data != null)
                Content = Encoding.UTF8.GetString(request.Data);
            Method = request.Method;
            Uri = request.Uri != null ? request.Uri.ToString() : null;
        }

        public byte[] GetData()
        {
            return Encoding.GetEncoding(ContentEncoding).GetBytes(Content);
        }

        [DataMember(Name = "contentEncoding")]
        public string ContentEncoding { get; set; }

        public string Content { get; set; }

        [DataMember(Name = "content")]
        public CDataWrapper ContentCData
        {
            get { return Content; }
            set { Content = value; }
        }

        [DataMember(Name = "headers")]
        public SerializableHeader[] Headers { get; set; }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }
    }
}
