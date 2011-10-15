using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Pipeline
{
    /// <summary>
    /// Participant in a <see cref="IRemoraOperation"/>
    /// </summary>
    public interface IPipelineComponent
    {
        string Id { get; set; }

        void Proceed(IPipelineComponentInvocation invocation);
    }
}
