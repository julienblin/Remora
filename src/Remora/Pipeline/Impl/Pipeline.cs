using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Pipeline.Impl
{
    public class Pipeline : IPipeline
    {
        public Pipeline(string id, string uriFilterRegex, string uriRewriteRegex)
            : this(id, uriFilterRegex, uriRewriteRegex, null)
        {
        }

        public Pipeline(string id, string uriFilterRegex, string uriRewriteRegex, IEnumerable<IPipelineComponent> components)
        {
            Id = id;
            UriFilterRegex = uriFilterRegex;
            UriRewriteRegex = uriRewriteRegex;
            _components = components == null ? new List<IPipelineComponent>() : new List<IPipelineComponent>(components);
        }

        public string Id { get; private set; }

        public string UriFilterRegex { get; private set; }

        public string UriRewriteRegex { get; private set; }

        private readonly IList<IPipelineComponent> _components;

        public IEnumerable<IPipelineComponent> Components
        {
            get { return _components; }
        }
    }
}
