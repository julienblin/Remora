using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora
{
    public interface IPipelineComponent
    {
        void Proceed(IPipelineComponentInvocation nextInvocation);

        IDictionary<string, string> Properties { get; }
    }
}
