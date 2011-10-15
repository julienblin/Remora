using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Pipeline;

namespace Remora.Tests
{
    public class TestComponentOne : IPipelineComponent
    {
        public string Id { get; set; }

        public void Proceed(IPipelineComponentInvocation invocation)
        {
            invocation.ProceedWithNextComponent();
        }
    }
}
