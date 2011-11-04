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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Remora.Core
{
    public interface IUniversalRequest
    {
        IEnumerable<string> AcceptTypes { get; }

        Encoding ContentEncoding { get; }

        long ContentLength { get; }

        string ContentType { get; }

        IDictionary<string, string> Cookies { get; }

        IDictionary<string, string> Headers { get; }

        string HttpMethod { get; }

        Stream InputStream { get; }

        bool IsAuthenticated { get; }

        bool IsLocal { get; }

        bool IsSecureConnection { get; }

        IDictionary<string, string> QueryString { get; }

        string RawUrl { get; }

        Uri Url { get; }

        Uri UrlReferrer { get; }

        string UserAgent { get; }

        string UserHostAddress { get; }

        string UserHostName { get; }

        IEnumerable<string> UserLanguages { get; }

        object OriginalRequest { get; }
    }
}