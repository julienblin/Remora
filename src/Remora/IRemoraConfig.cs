using System;
using System.Collections.Generic;

namespace Remora
{
    public interface IRemoraConfig
    {
        Type RequestProcessorType { get; }

        Type CategoryResolverType { get; }

        IEnumerable<Category> Categories { get; }
    }
}
