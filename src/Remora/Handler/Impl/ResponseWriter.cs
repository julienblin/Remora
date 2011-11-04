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
using Castle.Core.Logging;
using Remora.Core;
using Remora.Exceptions;

namespace Remora.Handler.Impl
{
    public class ResponseWriter : IResponseWriter
    {
        private readonly IExceptionFormatter _exceptionformatter;

        private ILogger _logger = NullLogger.Instance;

        public ResponseWriter(IExceptionFormatter exceptionformatter)
        {
            if (exceptionformatter == null) throw new ArgumentNullException("exceptionFormatter");
            Contract.EndContractBlock();

            _exceptionformatter = exceptionformatter;
        }

        /// <summary>
        ///   Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IResponseWriter Members

        public void Write(IRemoraOperation operation, IUniversalResponse response)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (response == null) throw new ArgumentNullException("response");
            Contract.EndContractBlock();

            if (operation.OnError)
            {
                Logger.ErrorFormat(operation.Exception,
                                   "There has been an error when processing request coming from {0}.",
                                   operation.IncomingUri);
                _exceptionformatter.WriteException(operation, response);
            }
            else
            {
                response.StatusCode = operation.Response.StatusCode;
                foreach (var header in operation.Response.HttpHeaders)
                {
                    response.SetHeader(header.Key, header.Value);
                }

                if (operation.Response.Data != null)
                {
                    response.OutputStream.Write(operation.Response.Data, 0, operation.Response.Data.Length);
                }
                response.OutputStream.Flush();
            }
        }

        #endregion
    }
}