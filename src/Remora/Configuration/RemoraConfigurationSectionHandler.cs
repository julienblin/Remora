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
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Xml;
using Remora.Configuration.Impl;

namespace Remora.Configuration
{
    public class RemoraConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public const string ConfigurationSectionName = @"remora";

        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
            var result = new RemoraConfig();

            foreach (XmlAttribute attr in section.Attributes)
            {
                switch (attr.Name.ToLowerInvariant())
                {
                    case "maxmessagesize":
                        int maxMessageSizeValue;
                        if (Int32.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                                           out maxMessageSizeValue))
                            result.MaxMessageSize = maxMessageSizeValue;
                        break;
                    default:
                        result.Properties.Add(attr.Name, attr.Value);
                        break;
                }
            }

            var pipelineNodes = section.SelectNodes("/" + ConfigurationSectionName + "/pipelines/pipeline");

            var pipelineDefs = new List<IPipelineDefinition>();
            foreach (XmlNode pipelineNode in pipelineNodes)
            {
                var pipelineDef = new PipelineDefinition();
                foreach (XmlAttribute attr in pipelineNode.Attributes)
                {
                    switch (attr.Name.ToLowerInvariant())
                    {
                        case "id":
                            pipelineDef.Id = attr.Value;
                            break;
                        case "filter":
                            pipelineDef.UriFilterRegex = attr.Value;
                            break;
                        case "rewrite":
                            pipelineDef.UriRewriteRegex = attr.Value;
                            break;
                        case "clientcertificatefilepath":
                            pipelineDef.ClientCertificateFilePath = attr.Value;
                            break;
                        case "clientcertificatepassword":
                            pipelineDef.ClientCertificatePassword = attr.Value;
                            break;
                        default:
                            pipelineDef.Properties.Add(attr.Name, attr.Value);
                            break;
                    }
                }

                pipelineDef.ComponentDefinitions = ParseComponents(pipelineNode);

                pipelineDefs.Add(pipelineDef);
            }

            result.PipelineDefinitions = pipelineDefs;

            return result;
        }

        #endregion

        public static IRemoraConfig GetConfiguration()
        {
            var config = (IRemoraConfig) ConfigurationManager.GetSection(ConfigurationSectionName);
            ;
            return config ?? new RemoraConfig();
        }

        private IEnumerable<IComponentDefinition> ParseComponents(XmlNode node)
        {
            var componentNodes = node.SelectNodes("component");

            foreach (XmlNode componentNode in componentNodes)
            {
                var cmpDef = new ComponentDefinition();
                foreach (XmlAttribute attr in componentNode.Attributes)
                {
                    switch (attr.Name.ToLowerInvariant())
                    {
                        case "id":
                            cmpDef.RefId = attr.Value;
                            break;
                        default:
                            cmpDef.Properties.Add(attr.Name, attr.Value);
                            break;
                    }
                }
                yield return cmpDef;
            }
        }
    }
}