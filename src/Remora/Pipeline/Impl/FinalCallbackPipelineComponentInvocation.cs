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
using Castle.Core.Logging;
using Remora.Core;

namespace Remora.Pipeline.Impl
{
    public class FinalCallbackPipelineComponentInvocation : IPipelineComponentInvocation
    {
        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public Action<IRemoraOperation> FinalCallback { get; set; }

        #region IPipelineComponentInvocation Members

        public IRemoraOperation Operation { get; set; }

        public IPipelineComponentInvocation NextInvocation { get; set; }

        public IPipelineComponentInvocation PreviousInvocation { get; set; }

        public virtual void BeginProcess()
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Calling NextInvocation[{0}].BeginProcess() on {1}...", NextInvocation, Operation);
            NextInvocation.BeginProcess();
        }

        public virtual void EndProcess()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("End of invocation chain, calling final callback...");
            FinalCallback.Invoke(Operation);
        }

        #endregion

        public override string ToString()
        {
            return "FinalCallback";
        }
    }
}
