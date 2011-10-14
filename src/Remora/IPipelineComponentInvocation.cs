using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora
{
    public interface IPipelineComponentInvocation
    {
        void Proceed();
    }
}
