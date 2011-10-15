using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Pipeline.Impl
{
    public class Pipeline : IPipeline
    {
        public Pipeline(string id, IEnumerable<IPipelineComponent> components)
        {
            Id = id;
            Components = components;
        }

        public string Id { get; private set; }

        public IEnumerable<IPipelineComponent> Components { get; private set; }
    }
}
