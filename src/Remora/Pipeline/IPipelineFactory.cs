using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineFactory
    {
        IPipeline Get(IRemoraOperation operation);
    }
}
