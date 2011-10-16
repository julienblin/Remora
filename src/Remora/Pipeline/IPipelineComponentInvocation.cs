using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineComponentInvocation
    {
        IRemoraOperation Operation { get; set; }

        IPipelineComponentInvocation NextInvocation { get; set; }

        IPipelineComponentInvocation PreviousInvocation { get; set; }

        void BeginProcess();
        void EndProcess();
    }
}
