using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Remora.Configuration.Impl;
using Remora.Exceptions;

namespace Remora.Configuration
{
    public class RemoraConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public const string ConfigurationSectionName = @"remora";

        public static IRemoraConfig GetConfiguration()
        {
            var config = (IRemoraConfig)ConfigurationManager.GetSection(ConfigurationSectionName);;
            return config ?? new RemoraConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var result = new RemoraConfig();

            foreach (XmlAttribute attr in section.Attributes)
            {
                switch (attr.Name.ToLowerInvariant())
                {
                    case "maxmessagesize":
                        int maxMessageSizeValue;
                        if (Int32.TryParse(attr.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxMessageSizeValue))
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
