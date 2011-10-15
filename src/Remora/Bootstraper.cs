using Castle.Facilities.Logging;
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

        public static void Init(params IRegistration[] registrations)
        {
            if (IsInitialized) return;

            lock (SyncRoot)
            {
                if (IsInitialized) return;

                InitContainer(registrations);

                IsInitialized = true;
            }
        }

        private static void InitContainer(params IRegistration[] registrations)
        {
            if (registrations.Length == 0)
            {
                Container = new WindsorContainer(new XmlInterpreter());
            }
            else
            {
                Container = new WindsorContainer();
                Container.Register(registrations);
            }
            Container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Log4net));
            Container.Register(
                RegisterIfMissing<IRemoraOperation, RemoraOperation>(true),
                RegisterIfMissing<IPipelineEngine, PipelineEngine>()
            );
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
