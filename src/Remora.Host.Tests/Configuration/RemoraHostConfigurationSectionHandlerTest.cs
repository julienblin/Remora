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

            Assert.That(config.ServiceName, Is.EqualTo("serviceName"));
            Assert.That(config.DisplayName, Is.EqualTo("displayName"));
            Assert.That(config.Description, Is.EqualTo("description"));

            Assert.That(config.ListenerConfigs.Count(), Is.EqualTo(2));
            Assert.That(config.ListenerConfigs.First().Prefix, Is.EqualTo("http://+:9091/"));
            Assert.That(config.ListenerConfigs.Skip(1).First().Prefix, Is.EqualTo("http://+:9092/"));
        }

        [Test]
        public void It_should_load_default_conf()
        {
            var config = RemoraHostConfigurationSectionHandler.GetConfiguration("default");

            Assert.That(config.ServiceName, Is.EqualTo(RemoraHostConfig.Defaults.ServiceName));
            Assert.That(config.DisplayName, Is.EqualTo(RemoraHostConfig.Defaults.DisplayName));
            Assert.That(config.Description, Is.EqualTo(RemoraHostConfig.Defaults.Description));
            Assert.That(config.ListenerConfigs.Count(), Is.EqualTo(0));
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
        public void It_should_validate_attributes_for_listener_nodes()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("badListenerNode"),
                Throws.Exception.TypeOf<ConfigurationErrorsException>()
                .With.InnerException.TypeOf<RemoraHostConfigException>()
                .And.Message.Contains("foo"));
        }

        [Test]
        public void It_should_validate_prefix_for_listener_nodes()
        {
            Assert.That(() => RemoraHostConfigurationSectionHandler.GetConfiguration("emptyPrefixForListenerNode"),
                Throws.Exception.TypeOf<ConfigurationErrorsException>()
                .With.InnerException.TypeOf<RemoraHostConfigException>()
                .And.Message.Contains("prefix"));
        }
    }
}
