﻿#region Licence

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
using System.Net;
using System.Text;
using System.Web;
using Remora.Core;

namespace Remora.Exceptions.Impl
{
    public class ExceptionFormatter : IExceptionFormatter
    {
        #region IExceptionFormatter Members

        public void WriteException(IRemoraOperation operation, IUniversalResponse response)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (response == null) throw new ArgumentNullException("response");
            Contract.EndContractBlock();

            switch (operation.Kind)
            {
                case RemoraOperationKind.Soap:
                    WriteSoap(operation, response);
                    break;
                default:
                    WriteHtmlException(operation.Exception, response);
                    break;
            }
        }

        public void WriteHtmlException(Exception exception, IUniversalResponse response)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            if (response == null) throw new ArgumentNullException("response");
            Contract.EndContractBlock();

            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;

            var content = response.ContentEncoding.GetBytes(string.Format(ErrorResources.GenericHtmlError,
                                         exception.GetType().Name.Replace("Exception", ""), exception.Message));

            response.OutputStream.Write(content, 0, content.Length);
            response.OutputStream.Flush();
        }

        #endregion

        protected virtual void WriteSoap(IRemoraOperation operation, IUniversalResponse response)
        {
            response.ContentType = "text/xml";
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentEncoding = Encoding.UTF8;

            var content = response.ContentEncoding.GetBytes(string.Format(ErrorResources.SoapError,
                                         operation.Exception.GetType().Name.Replace("Exception", ""),
                                         operation.Exception.Message));

            response.OutputStream.Write(content, 0, content.Length);
            response.OutputStream.Flush();
        }
    }
}