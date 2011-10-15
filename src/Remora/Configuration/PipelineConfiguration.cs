using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration
{
    public class PipelineConfiguration
    {
        public string Id { get; set; }

        public string UriFilterRegex { get; set; }

        public string UriRewriteRegex { get; set; }

        public IList<string> Components { get; set; }
    }
}
