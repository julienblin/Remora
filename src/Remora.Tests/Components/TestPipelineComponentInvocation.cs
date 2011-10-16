using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Pipeline;

namespace Remora.Tests.Components
{
    public class TestPipelineComponentInvocation : IPipelineComponentInvocation
    {
        public TestPipelineComponentInvocation()
        {
            Operation = new RemoraOperation();
            Pipeline = new Remora.Pipeline.Impl.Pipeline("test", new IPipelineComponent[0]);
        }

        public IRemoraOperation Operation { get; private set; }
        
        public IPipeline Pipeline { get; private set; }
        
        public void ProceedWithNextComponent()
        {
        }
    }
}
