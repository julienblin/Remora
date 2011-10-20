using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remora.Core.Serialization
{
    [DataContract(Name = "response", Namespace = SerializationConstants.Namespace)]
    public class SerializableResponse
    {
        public SerializableResponse()
        {
        }

        public SerializableResponse(IRemoraResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");
            Contract.EndContractBlock();

            Headers = response.HttpHeaders.Select(k => new SerializableHeader(k.Key, k.Value)).ToArray();
            ContentEncoding = response.ContentEncoding != null ? response.ContentEncoding.HeaderName : null;
            Content = Encoding.UTF8.GetString(response.Data);
            StatusCode = response.StatusCode;
            Uri = response.Uri != null ? response.Uri.ToString() : null;
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

        [DataMember(Name = "statusCode")]
        public int StatusCode { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }
    }
}
