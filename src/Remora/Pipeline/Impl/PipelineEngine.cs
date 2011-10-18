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
using System.Diagnostics.Contracts;
using System.Linq;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Remora.Components;
using Remora.Core;

namespace Remora.Pipeline.Impl
{
    public class PipelineEngine : IPipelineEngine
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

        public IKernel Kernel { get; set; }

        #region IPipelineEngine Members

        public void RunAsync(IRemoraOperation operation, IPipeline pipeline, Action<IRemoraOperation> callback)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (pipeline == null) throw new ArgumentNullException("pipeline");
            if (callback == null) throw new ArgumentNullException("callback");
            Contract.EndContractBlock();

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Running operation {0} on pipeline {1}...", operation, pipeline.Id);

            var topInvocation = BuildInvocations(operation, pipeline, callback);
            topInvocation.BeginProcess();
        }

        #endregion

        private IPipelineComponentInvocation BuildInvocations(IRemoraOperation operation, IPipeline pipeline, Action<IRemoraOperation> callback)
        {
            var currentInvocation = new PipelineComponentInvocation { Logger = Logger, Operation = operation, Component = Kernel.Resolve<IPipelineComponent>(Sender.ComponentId)};

            var pipelineDefs = pipeline.Definition != null ? pipeline.Definition.ComponentDefinitions.Reverse().ToArray() : null;

            int currentIndex = 0;
            foreach (var pipelineComponent in pipeline.Components.Reverse())
            {
                if (pipelineComponent != null)
                {
                    var previousInvocation = currentInvocation;
                    currentInvocation = new PipelineComponentInvocation
                    {
                        Logger              = Logger,
                        Operation           = operation,
                        ComponentDefinition = (pipelineDefs != null && pipelineDefs.Length > currentIndex) ? pipelineDefs[currentIndex] : null,
                        Component           = pipelineComponent,
                        NextInvocation      = currentInvocation
                    };
                    previousInvocation.PreviousInvocation = currentInvocation;
                }
                ++currentIndex;
            }

            var finalInvocation = new FinalCallbackPipelineComponentInvocation { Logger = Logger, Operation = operation, NextInvocation = currentInvocation, FinalCallback = callback };
            currentInvocation.PreviousInvocation = finalInvocation;

            return finalInvocation;
        }
    }
}
