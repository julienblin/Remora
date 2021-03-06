﻿#region Licence

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

using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Remora.Host.Configuration;
using Remora.Host.Configuration.Impl;
using Remora.Host.Exceptions;

namespace Remora.Host.Tests.Configuration
{
    [TestFixture]
    public class RemoraHostConfigurationSectionHandlerTest : BaseTest
    {
        [Test]
        public void It_should_load_appropriate_configuration()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration();

            Assert.That(config.ServiceConfig.ServiceName, Is.EqualTo("serviceName"));
            Assert.That(config.ServiceConfig.DisplayName, Is.EqualTo("displayName"));
            Assert.That(config.ServiceConfig.Description, Is.EqualTo("description"));
            Assert.That(config.ServiceConfig.RunAs, Is.EqualTo(ServiceConfigRunAs.LocalSystem));

            Assert.That(config.BindingConfigs.Count(), Is.EqualTo(2));
            Assert.That(config.BindingConfigs.First().Prefix, Is.EqualTo("http://+:9091/"));
            Assert.That(config.BindingConfigs.Skip(1).First().Prefix, Is.EqualTo("http://+:9092/"));

            Assert.That(config.JobsConfig.JobConfigs.Count(), Is.EqualTo(1));
            Assert.That(config.JobsConfig.JobConfigs.First().Cron, Is.EqualTo("0 0/5 * * * ?"));
            Assert.That(config.JobsConfig.JobConfigs.First().Name, Is.EqualTo("SampleJob"));
        }

        [Test]
        public void It_should_load_default_conf()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration("default");

            Assert.That(config.ServiceConfig.ServiceName, Is.EqualTo(ServiceConfig.Defaults.ServiceName));
            Assert.That(config.ServiceConfig.DisplayName, Is.EqualTo(ServiceConfig.Defaults.DisplayName));
            Assert.That(config.ServiceConfig.Description, Is.EqualTo(ServiceConfig.Defaults.Description));
            Assert.That(config.ServiceConfig.RunAs, Is.EqualTo(ServiceConfig.Defaults.RunAs));
            Assert.That(config.BindingConfigs.Count(), Is.EqualTo(0));
        }

        [Test]
        public void It_should_load_local_network_service()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration("networkService");
            Assert.That(config.ServiceConfig.RunAs, Is.EqualTo(ServiceConfigRunAs.NetworkService));
        }

        [Test]
        public void It_should_load_local_service()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration("localService");
            Assert.That(config.ServiceConfig.RunAs, Is.EqualTo(ServiceConfigRunAs.LocalService));
        }

        [Test]
        public void It_should_load_local_system()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration("localSystem");
            Assert.That(config.ServiceConfig.RunAs, Is.EqualTo(ServiceConfigRunAs.LocalSystem));
        }

        [Test]
        public void It_should_load_runas_user()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration("runAsUser");
            Assert.That(config.ServiceConfig.RunAs, Is.EqualTo(ServiceConfigRunAs.User));
            Assert.That(config.ServiceConfig.Username, Is.EqualTo("foo"));
            Assert.That(config.ServiceConfig.Password, Is.EqualTo("bar"));
        }

        [Test]
        public void It_should_validate_attributes_for_binding_nodes()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badBindingNode"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("foo"));
        }

        [Test]
        public void It_should_validate_attributes_for_service_node()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badServiceNode"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("foo"));
        }

        [Test]
        public void It_should_validate_job_attributes()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badAttributeInJob"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("foo"));
        }

        [Test]
        public void It_should_validate_job_cron()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("missingCronInJob"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("cron"));
        }

        [Test]
        public void It_should_validate_job_name()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("missingNameInJob"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("name"));
        }

        [Test]
        public void It_should_validate_prefix_for_binding_nodes()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("emptyPrefixForBindingNode"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("prefix"));
        }

        [Test]
        public void It_should_validate_runas_user()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badRunAsUser"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("username"));
        }

        [Test]
        public void It_should_validate_runas_value()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badRunAsValue"),
                        Throws.Exception.TypeOf<ConfigurationErrorsException>()
                            .With.InnerException.TypeOf<RemoraHostConfigException>()
                            .And.Message.Contains("foo"));
        }
    }
}