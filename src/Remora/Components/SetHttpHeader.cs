using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Exceptions;
using Remora.Pipeline;

namespace Remora.Components
{
    public class SetHttpHeader : AbstractPipelineComponent
    {
        public const string ComponentId = @"setHttpHeader";

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            if (!componentDefinition.Properties.ContainsKey("name") || !componentDefinition.Properties.ContainsKey("value"))
            {
                throw new SetHttpHeaderException(string.Format("Unable to set http header for operation {0}: missing name or value attribute in component configuration.", operation));
            }
            else
            {
                var name = componentDefinition.Properties["name"];
                var value = componentDefinition.Properties["value"];

                if(Logger.IsDebugEnabled)
                    Logger.DebugFormat("Setting header {0}={1}", name, value);

                operation.Request.HttpHeaders[name] = value;
            }

            callback(true);
        }
    }
}
