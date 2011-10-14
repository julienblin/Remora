using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora
{
    public abstract class AbstractPipelineComponent : IPipelineComponent
    {
        protected AbstractPipelineComponent()
        {
            Properties = new Dictionary<string, string>();
        }

        public abstract void Proceed(IPipelineComponentInvocation nextInvocation);

        public IDictionary<string, string> Properties { get; private set; }
    }
}
