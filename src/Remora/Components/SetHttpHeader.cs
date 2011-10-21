#region Licence

// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
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
        ///   Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition,
                                               Action<bool> callback)
        {
            if (!componentDefinition.Properties.ContainsKey("name") ||
                !componentDefinition.Properties.ContainsKey("value"))
            {
                throw new SetHttpHeaderException(
                    string.Format(
                        "Unable to set http header for operation {0}: missing name or value attribute in component configuration.",
                        operation));
            }
            else
            {
                var name = componentDefinition.Properties["name"];
                var value = componentDefinition.Properties["value"];

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Setting header {0}={1}", name, value);

                operation.Request.HttpHeaders[name] = value;
            }

            callback(true);
        }
    }
}