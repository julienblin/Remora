using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Remora.Core.Impl
{
    public class UniversalResponse : IUniversalResponse
    {
        private readonly HttpResponseBase _httpResponse;
        private readonly HttpListenerResponse _httpListenerResponse;
        private readonly Mode _mode;

        public UniversalResponse(HttpResponse response)
            : this(new HttpResponseWrapper(response))
        {
        }

        public UniversalResponse(HttpResponseBase response)
        {
            _httpResponse = response;
            _mode = Mode.HttpResponse;
        }

        public UniversalResponse(HttpListenerResponse response)
        {
            _httpListenerResponse = response;
            _mode = Mode.HttpListenerResponse;
        }

        public Encoding ContentEncoding
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        return _httpResponse.ContentEncoding;
                    case Mode.HttpListenerResponse:
                        return _httpListenerResponse.ContentEncoding;
                    default:
                        throw new NotSupportedException();
                }
            }

            set
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        _httpResponse.ContentEncoding = value;
                        break;
                    case Mode.HttpListenerResponse:
                        _httpListenerResponse.ContentEncoding = value;
                        break;
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
                    case Mode.HttpResponse:
                        return _httpResponse.ContentType;
                    case Mode.HttpListenerResponse:
                        return _httpListenerResponse.ContentType;
                    default:
                        throw new NotSupportedException();
                }
            }

            set
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        _httpResponse.ContentType = value;
                        break;
                    case Mode.HttpListenerResponse:
                        _httpListenerResponse.ContentType = value;
                        break;
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
                    case Mode.HttpResponse:
                        if (_httpResponse.Headers == null)
                            return null;
                        return _httpResponse.Headers.Cast<string>().ToDictionary(x => x, x => _httpResponse.Headers[x]);
                    case Mode.HttpListenerResponse:
                        if (_httpListenerResponse.Headers == null)
                            return null;
                        return _httpListenerResponse.Headers.Cast<string>().ToDictionary(x => x, x => _httpListenerResponse.Headers[x]);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void SetHeader(string name, string value)
        {
            if (!name.Equals("Content-Length", StringComparison.InvariantCultureIgnoreCase))
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        _httpResponse.AppendHeader(name, value);
                        break;
                    case Mode.HttpListenerResponse:

                        _httpListenerResponse.AppendHeader(name, value);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public Stream OutputStream
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        return _httpResponse.OutputStream;
                    case Mode.HttpListenerResponse:
                        return _httpListenerResponse.OutputStream;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string RedirectLocation
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        return _httpResponse.RedirectLocation;
                    case Mode.HttpListenerResponse:
                        return _httpListenerResponse.RedirectLocation;
                    default:
                        throw new NotSupportedException();
                }
            }

            set
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        _httpResponse.RedirectLocation = value;
                        break;
                    case Mode.HttpListenerResponse:
                        _httpListenerResponse.RedirectLocation = value;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public int StatusCode
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        return _httpResponse.StatusCode;
                    case Mode.HttpListenerResponse:
                        return _httpListenerResponse.StatusCode;
                    default:
                        throw new NotSupportedException();
                }
            }

            set
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        _httpResponse.StatusCode = value;
                        break;
                    case Mode.HttpListenerResponse:
                        _httpListenerResponse.StatusCode = value;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string StatusDescription
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        return _httpResponse.StatusDescription;
                    case Mode.HttpListenerResponse:
                        return _httpListenerResponse.StatusDescription;
                    default:
                        throw new NotSupportedException();
                }
            }

            set
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        _httpResponse.StatusDescription = value;
                        break;
                    case Mode.HttpListenerResponse:
                        _httpListenerResponse.StatusDescription = value;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public object OriginalResponse
        {
            get
            {
                switch (_mode)
                {
                    case Mode.HttpResponse:
                        return _httpResponse;
                    case Mode.HttpListenerResponse:
                        return _httpListenerResponse;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private enum Mode
        {
            HttpResponse,
            HttpListenerResponse
        }
    }
}
