#region License
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

namespace Remora.Core.Impl
{
    public class RemoraOperation : IRemoraOperation
    {
        public RemoraOperation()
        {
            OperationId = Guid.NewGuid();
            IncomingRequest = new RemoraRequest();
            Request = new RemoraRequest();
            Response = new RemoraResponse();
        }

        public string IncomingContentType { get; set; }

        #region IRemoraOperation Members

        public Guid OperationId { get; private set; }

        public IRemoraRequest IncomingRequest { get; private set; }

        public IRemoraRequest Request { get; private set; }

        public IRemoraResponse Response { get; private set; }

        public RemoraOperationKind Kind { get; set; }

        public Exception Exception { get; set; }

        public bool OnError { get { return Exception != null; } }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}({1})", OperationId, IncomingRequest.Uri);
        }
    }
}
