using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineComponentInvocation
    {
        IRemoraOperation Operation { get; }

        IPipeline Pipeline { get; }

        void ProceedWithNextComponent();
    }
}
