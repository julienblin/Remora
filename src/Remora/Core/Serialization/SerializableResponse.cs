#region Licence

// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
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
            if (response.Data != null)
                Content = Encoding.UTF8.GetString(response.Data);
            StatusCode = response.StatusCode;
            Uri = response.Uri != null ? response.Uri.ToString() : null;
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

        public byte[] GetData()
        {
            return Encoding.GetEncoding(ContentEncoding).GetBytes(Content);
        }
    }
}