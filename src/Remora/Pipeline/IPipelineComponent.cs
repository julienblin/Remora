using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineComponent
    {
        void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback);

        void EndAsyncProcess(IRemoraOperation operation, Action callback);
    }
}
