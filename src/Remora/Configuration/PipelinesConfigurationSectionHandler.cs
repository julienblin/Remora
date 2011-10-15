using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using Remora.Exceptions;

namespace Remora.Configuration
{
    public class PipelinesConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public const string ConfigurationSectionName = @"pipelines";

        public static IEnumerable<PipelineConfiguration> GetConfiguration()
        {
            var section = ConfigurationManager.GetSection(ConfigurationSectionName);
            if(section == null)
                throw new InvalidConfigurationException("Unable to find configuration section " + ConfigurationSectionName + " in application configuration file.");
            
            return (IEnumerable<PipelineConfiguration>)section;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var result = new List<PipelineConfiguration>();

            var pipelineNodes = section.SelectNodes("/" + ConfigurationSectionName + "/pipeline");

            foreach (XmlNode pipelineNode in pipelineNodes)
            {
                var pipelineConfig = new PipelineConfiguration
                                         {
                                             Id = SafelyReadAttribute(pipelineNode, "id"),
                                             UriFilterRegex = SafelyReadAttribute(pipelineNode, "filter"),
                                             UriRewriteRegex = SafelyReadAttribute(pipelineNode, "rewrite")
                                         };

                if (string.IsNullOrWhiteSpace(pipelineConfig.Id))
                    throw new InvalidConfigurationException("One of the pipeline is missing the mandatory 'id' attribute, or its value is empty. Every pipeline must be identified by an id.");

                pipelineConfig.Components = ParseComponents(pipelineNode);

                result.Add(pipelineConfig);
            }

            return result;
        }

        private IList<string> ParseComponents(XmlNode node)
        {
            var componentNodes = node.SelectNodes("component");

            return (from XmlNode componentNode in componentNodes
                    select SafelyReadAttribute(componentNode, "id") into id
                    where !string.IsNullOrWhiteSpace(id)
                    select id).ToList();
        }

        private string SafelyReadAttribute(XmlNode node, string attributeName)
        {
            var attribute = node.Attributes[attributeName];
            return attribute != null ? attribute.Value : null;
        }
    }
}
