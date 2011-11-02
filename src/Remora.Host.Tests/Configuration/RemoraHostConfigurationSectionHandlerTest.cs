using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
        public void It_should_validate_attributes_for_service_node()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badServiceNode"),
                Throws.Exception.TypeOf<ConfigurationErrorsException>()
                .With.InnerException.TypeOf<RemoraHostConfigException>()
                .And.Message.Contains("foo"));
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
        public void It_should_validate_prefix_for_binding_nodes()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("emptyPrefixForBindingNode"),
                Throws.Exception.TypeOf<ConfigurationErrorsException>()
                .With.InnerException.TypeOf<RemoraHostConfigException>()
                .And.Message.Contains("prefix"));
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
        public void It_should_load_local_network_service()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration("networkService");
            Assert.That(config.ServiceConfig.RunAs, Is.EqualTo(ServiceConfigRunAs.NetworkService));
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
        public void It_should_validate_runas_value()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badRunAsValue"),
                Throws.Exception.TypeOf<ConfigurationErrorsException>()
                .With.InnerException.TypeOf<RemoraHostConfigException>()
                .And.Message.Contains("foo"));
        }

        [Test]
        public void It_should_validate_runas_user()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badRunAsUser"),
                Throws.Exception.TypeOf<ConfigurationErrorsException>()
                .With.InnerException.TypeOf<RemoraHostConfigException>()
                .And.Message.Contains("username"));
        }
    }
}
