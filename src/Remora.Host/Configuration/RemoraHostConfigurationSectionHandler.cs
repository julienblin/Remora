using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
            LoadBindings(section, result);
            LoadJobs(section, result);

            return result;
        }

        private void LoadJobs(XmlNode section, RemoraHostConfig result)
        {
           var jobsNode = section.SelectSingleNode("//jobs");
           if (jobsNode != null)
           {
               var jobsConfig = new JobsConfig();

               var jobNodes = jobsNode.SelectNodes("job");
               foreach (XmlNode jobNode in jobNodes)
               {
                   var jobConfig = new JobConfig();
                   var jobConfigList = new List<JobConfig>();

                   foreach (XmlAttribute attr in jobNode.Attributes)
                   {
                       switch (attr.Name.ToLowerInvariant())
                       {
                           case "cron":
                               jobConfig.Cron = attr.Value;
                               break;
                           case "name":
                               jobConfig.Name = attr.Value;
                               break;
                           default:
                               throw new RemoraHostConfigException(string.Format("Unknown attribute for job node: {0}", attr.Name));
                       }
                   }

                   if (string.IsNullOrWhiteSpace(jobConfig.Cron))
                       throw new RemoraHostConfigException("Missing required attribute cron for a job.");

                   if (string.IsNullOrWhiteSpace(jobConfig.Name))
                       throw new RemoraHostConfigException("Missing required attribute name for a job.");

                   jobConfigList.Add(jobConfig);
                   jobsConfig.JobConfigs = jobConfigList;
               }

               result.JobsConfig = jobsConfig;
           }
        }

        private void LoadBindings(XmlNode section, RemoraHostConfig result)
        {
            var bindingsNode = section.SelectSingleNode("//bindings");
            if (bindingsNode != null)
            {
                var bindings = new List<IBindingConfig>();

                var bindingNodes = bindingsNode.SelectNodes("binding");
                foreach (XmlNode bindingNode in bindingNodes)
                {
                    var bindingConfig = new BindingConfig();

                    foreach (XmlAttribute attr in bindingNode.Attributes)
                    {
                        switch (attr.Name.ToLowerInvariant())
                        {
                            case "prefix":
                                bindingConfig.Prefix = attr.Value;
                                break;
                            default:
                                throw new RemoraHostConfigException(string.Format("Unknown attribute for binding node: {0}", attr.Name));
                        }
                    }

                    if(string.IsNullOrWhiteSpace(bindingConfig.Prefix))
                        throw new RemoraHostConfigException("Missing required attribute prefix for a binding.");
                    
                    bindings.Add(bindingConfig);
                }

                result.BindingConfigs = bindings;
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
