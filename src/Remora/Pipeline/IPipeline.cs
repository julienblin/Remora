using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Pipeline
{
    public interface IPipeline
    {
        string Id { get; }

        IEnumerable<IPipelineComponent> Components { get; }
    }
}
