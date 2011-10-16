using System.Configuration;
using Castle.Facilities.Logging;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Pipeline;
using Remora.Pipeline.Impl;

namespace Remora
{
    public sealed class Bootstraper
    {
        private static readonly object SyncRoot = new object();

        public static void Init(IWindsorContainer container = null)
        {
            if (IsInitialized) return;

            lock (SyncRoot)
            {
                if (IsInitialized) return;

                Container = container ?? InitContainer();

                IsInitialized = true;
            }
        }

        private static IWindsorContainer InitContainer()
        {
            var container = ConfigurationManager.GetSection("castle") != null ? new WindsorContainer(new XmlInterpreter()) : new WindsorContainer();
            container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Log4net));
            container.AddFacility<StartableFacility>();
            container.Register(
                RegisterIfMissing<IRemoraOperation, RemoraOperation>(true),
                RegisterIfMissing<IRemoraOperationFactory, RemoraOperationFactory>(),
                RegisterIfMissing<IPipelineFactory, PipelineFactory>(),
                RegisterIfMissing<IPipelineEngine, PipelineEngine>()
            );

            return container;
        }

        private static ComponentRegistration<TService> RegisterIfMissing<TService, TImpl>(bool transient = false)
             where TImpl : TService
        {
            if (transient)
            {
                return Component.For<TService>()
                                .ImplementedBy<TImpl>()
                                .LifeStyle.Transient
                                .Unless((k, m) => k.HasComponent(typeof(TService)));
            }
            else
            {
                return Component.For<TService>()
                                .ImplementedBy<TImpl>()
                                .Unless((k, m) => k.HasComponent(typeof(TService)));
            }
        }

        public static bool IsInitialized { get; private set; }

        public static IWindsorContainer Container { get; private set; }
    }
}
