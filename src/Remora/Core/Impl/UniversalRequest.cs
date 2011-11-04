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
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Remora.Core.Impl
{
    public class UniversalRequest : IUniversalRequest
    {
        private readonly HttpListenerRequest _httpListenerRequest;
        private readonly HttpRequestBase _httpRequest;
        private readonly Mode _mode;

        public UniversalRequest(HttpRequest request)
            : this(new HttpRequestWrapper(request))
        {
        }

        public UniversalRequest(HttpRequestBase request)
        {
            _httpRequest = request;
            _mode = Mode.HttpRequest;
        }

        public UniversalRequest(HttpListenerRequest request)
        {
            _httpListenerRequest = request;
            _mode = Mode.HttpListenerRequest;
        }

        #region IUniversalRequest Members

        public IEnumerable<string> AcceptTypes
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.AcceptTypes;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.AcceptTypes;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public Encoding ContentEncoding
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.ContentEncoding;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.ContentEncoding;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public long ContentLength
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.ContentLength;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.ContentLength64;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string ContentType
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.ContentType;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.ContentType;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public IDictionary<string, string> Cookies
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        if (_httpRequest.Cookies == null)
                            return null;
                        return _httpRequest.Cookies.Cast<string>().ToDictionary(x => x,
                                                                                x => _httpRequest.Cookies[x].Value,
                                                                                StringComparer.
                                                                                    InvariantCultureIgnoreCase);
                    case Mode.HttpListenerRequest:
                        if (_httpListenerRequest.Cookies == null)
                            return null;
                        return _httpListenerRequest.Cookies.Cast<string>().ToDictionary(x => x,
                                                                                        x =>
                                                                                        _httpListenerRequest.Cookies[x].
                                                                                            Value,
                                                                                        StringComparer.
                                                                                            InvariantCultureIgnoreCase);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public IDictionary<string, string> Headers
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        if (_httpRequest.Headers == null)
                            return null;
                        return _httpRequest.Headers.Cast<string>().ToDictionary(x => x, x => _httpRequest.Headers[x]);
                    case Mode.HttpListenerRequest:
                        if (_httpListenerRequest.Headers == null)
                            return null;
                        return _httpListenerRequest.Headers.Cast<string>().ToDictionary(x => x,
                                                                                        x =>
                                                                                        _httpListenerRequest.Headers[x]);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string HttpMethod
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.HttpMethod;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.HttpMethod;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public Stream InputStream
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.InputStream;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.InputStream;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.IsAuthenticated;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.IsAuthenticated;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public bool IsLocal
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.IsLocal;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.IsLocal;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public bool IsSecureConnection
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.IsSecureConnection;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.IsSecureConnection;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public IDictionary<string, string> QueryString
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        if (_httpRequest.QueryString == null)
                            return null;
                        return _httpRequest.QueryString.Cast<string>().ToDictionary(x => x,
                                                                                    x => _httpRequest.QueryString[x]);
                    case Mode.HttpListenerRequest:
                        if (_httpListenerRequest.QueryString == null)
                            return null;
                        return _httpListenerRequest.QueryString.Cast<string>().ToDictionary(x => x,
                                                                                            x =>
                                                                                            _httpListenerRequest.
                                                                                                QueryString[x]);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string RawUrl
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.RawUrl;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.RawUrl;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public Uri Url
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.Url;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.Url;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public Uri UrlReferrer
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.UrlReferrer;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.UrlReferrer;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string UserAgent
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.UserAgent;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.UserAgent;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string UserHostAddress
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.UserHostAddress;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.UserHostAddress;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string UserHostName
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.UserHostName;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.UserHostName;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public IEnumerable<string> UserLanguages
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest.UserLanguages;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest.UserLanguages;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public object OriginalRequest
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpRequest:
                        return _httpRequest;
                    case Mode.HttpListenerRequest:
                        return _httpListenerRequest;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion

        #region Nested type: Mode

        private enum Mode
        {
            HttpRequest,
            HttpListenerRequest
        }

        #endregion
    }
}