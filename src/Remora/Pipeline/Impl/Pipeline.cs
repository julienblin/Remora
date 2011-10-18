using System.Collections.Generic;

namespace Remora.Pipeline.Impl
{
    public class Pipeline : IPipeline
    {
        public Pipeline(string id, IEnumerable<IPipelineComponent> components)
        {
            Id = id;
            Components = components;
        }

        #region IPipeline Members

        public string Id { get; private set; }

        public IEnumerable<IPipelineComponent> Components { get; private set; }

        #endregion
    }
}
