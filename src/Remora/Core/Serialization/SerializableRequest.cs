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

        public byte[] GetData()
        {
            return Encoding.GetEncoding(ContentEncoding).GetBytes(Content);
        }
    }
}