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
                var serviceConfig = new ServiceConfig();

                foreach (XmlAttribute attr in serviceNode.Attributes)
                {
                    switch (attr.Name.ToLowerInvariant())
                    {
                        case "name":
                            serviceConfig.ServiceName = attr.Value;
                            break;
                        case "displayname":
                            serviceConfig.DisplayName = attr.Value;
                            break;
                        case "description":
                            serviceConfig.Description = attr.Value;
                            break;
                        case "runas":
                            switch (attr.Value.ToLowerInvariant())
                            {
                                case "localservice":
                                    serviceConfig.RunAs = ServiceConfigRunAs.LocalService;
                                    break;
                                case "localsystem":
                                    serviceConfig.RunAs = ServiceConfigRunAs.LocalSystem;
                                    break;
                                case "networkservice":
                                    serviceConfig.RunAs = ServiceConfigRunAs.NetworkService;
                                    break;
                                case "user":
                                    serviceConfig.RunAs = ServiceConfigRunAs.User;
                                    break;
                                default:
                                    throw new RemoraHostConfigException(string.Format("Unknown runas option for service node: {0}", attr.Value));
                            }
                            break;
                        case "username":
                            serviceConfig.Username = attr.Value;
                            break;
                        case "password":
                            serviceConfig.Password= attr.Value;
                            break;
                        default:
                            throw new RemoraHostConfigException(string.Format("Unknown attribute for service node: {0}", attr.Name));
                    }
                }

                if((serviceConfig.RunAs == ServiceConfigRunAs.User) && string.IsNullOrWhiteSpace(serviceConfig.Username))
                    throw new RemoraHostConfigException("Missing mandatory username value for service node when runAs=user.");

                result.ServiceConfig = serviceConfig;
            }
        }
    }
}
