using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using Remora.Host.Configuration.Impl;
using Remora.Host.Exceptions;

namespace Remora.Host.Configuration
{
    public class RemoraHostConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public const string ConfigurationSectionName = @"remora.host";

        public static IRemoraHostConfig GetConfiguration(string sectionName = ConfigurationSectionName)
        {
            var config = (IRemoraHostConfig)ConfigurationManager.GetSection(sectionName);
            return config ?? new RemoraHostConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var result = new RemoraHostConfig();

            LoadServiceNode(section, result);
            LoadListeners(section, result);

            return result;
        }

        private void LoadListeners(XmlNode section, RemoraHostConfig result)
        {
            var listenersNode = section.SelectSingleNode("//listeners");
            if (listenersNode != null)
            {
                var listeners = new List<IListenerConfig>();

                var listenerNodes = listenersNode.SelectNodes("listener");
                foreach (XmlNode listenerNode in listenerNodes)
                {
                    var listenerConfig = new ListenerConfig();

                    foreach (XmlAttribute attr in listenerNode.Attributes)
                    {
                        switch (attr.Name.ToLowerInvariant())
                        {
                            case "prefix":
                                listenerConfig.Prefix = attr.Value;
                                break;
                            default:
                                throw new RemoraHostConfigException(string.Format("Unknown attribute for listener node: {0}", attr.Name));
                        }
                    }

                    if(string.IsNullOrWhiteSpace(listenerConfig.Prefix))
                        throw new RemoraHostConfigException("Missing required attribute prefix for a listener.");
                    
                    listeners.Add(listenerConfig);
                }

                result.ListenerConfigs = listeners;
            }
        }

        private void LoadServiceNode(XmlNode section, RemoraHostConfig result)
        {
            var serviceNode = section.SelectSingleNode("//service");
            if (serviceNode != null)
            {
                foreach (XmlAttribute attr in serviceNode.Attributes)
                {
                    switch (attr.Name.ToLowerInvariant())
                    {
                        case "name":
                            result.ServiceName = attr.Value;
                            break;
                        case "displayname":
                            result.DisplayName = attr.Value;
                            break;
                        case "description":
                            result.Description = attr.Value;
                            break;
                        default:
                            throw new RemoraHostConfigException(string.Format("Unknown attribute for service node: {0}", attr.Name));
                    }
                }
            }
        }
    }
}
