using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Impl
{
    public class RemoraConfig : IRemoraConfig
    {
        public Type RequestProcessorType { get; set; }

        public Type CategoryResolverType { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}
