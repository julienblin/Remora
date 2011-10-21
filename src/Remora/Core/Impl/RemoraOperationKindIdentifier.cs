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

namespace Remora.Core.Impl
{
    public class RemoraOperationKindIdentifier : IRemoraOperationKindIdentifier
    {
        private ILogger _logger = NullLogger.Instance;

        /// <summary>
        ///   Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IRemoraOperationKindIdentifier Members

        public RemoraOperationKind Identify(IRemoraOperation operation)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            Contract.EndContractBlock();

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Identifying operation {0}...", operation);

            if (operation.Request.HttpHeaders.ContainsKey("SOAPAction"))
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Operation {0} is kind of {1}.", operation, RemoraOperationKind.Soap);
                return RemoraOperationKind.Soap;
            }


            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Unable to identify operation {0}.", operation);
            return RemoraOperationKind.Unknown;
        }

        #endregion
    }
}