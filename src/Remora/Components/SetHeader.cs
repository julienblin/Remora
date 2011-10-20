using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Configuration;
using Remora.Core;
using Remora.Pipeline;

namespace Remora.Components
{
    public class SetHeader : AbstractPipelineComponent
    {
        public const string ComponentId = @"setHeader";

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            var name = componentDefinition.Properties["name"];
            var value = componentDefinition.Properties["value"];

            operation.Request.HttpHeaders[name] = value;
            callback(true);
        }
    }
}
