#region License
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
using Castle.Facilities.FactorySupport;
using Castle.Facilities.Logging;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Remora.Components;
using Remora.Configuration;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Exceptions.Impl;
using Remora.Handler;
using Remora.Handler.Impl;
using Remora.Pipeline;
using Remora.Pipeline.Impl;
using Remora.Transformers;
using Remora.Transformers.Impl;

namespace Remora
{
    public sealed class Bootstraper
    {
        private static readonly object SyncRoot = new object();
        public static bool IsInitialized { get; private set; }

        public static IWindsorContainer Container { get; private set; }

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
            container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Log4net).WithAppConfig());
            container.AddFacility<StartableFacility>();
            container.AddFacility<FactorySupportFacility>();
            container.Register(
                RegisterIfMissing<IRemoraOperation, RemoraOperation>(true),
                RegisterIfMissing<IRemoraOperationKindIdentifier, RemoraOperationKindIdentifier>(),
                RegisterIfMissing<IRemoraOperationFactory, RemoraOperationFactory>(),
                RegisterIfMissing<IPipelineFactory, PipelineFactory>(),
                RegisterIfMissing<IPipelineEngine, PipelineEngine>(),
                RegisterIfMissing<IExceptionFormatter, ExceptionFormatter>(),
                RegisterIfMissing<IResponseWriter, ResponseWriter>(),

                Component.For<IRemoraConfig>()
                    .UsingFactoryMethod(RemoraConfigurationSectionHandler.GetConfiguration),

                RegisterPipelineComponent<Sender>(Sender.SenderComponentId),
                RegisterPipelineComponent<Recorder>(Recorder.ComponentId),
                
                RegisterIfMissing<ISoapTransformer, SoapTransformer>()
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

        private static ComponentRegistration<IPipelineComponent> RegisterPipelineComponent<TImpl>(string id)
            where TImpl : IPipelineComponent
        {
            return Component.For<IPipelineComponent>()
                .ImplementedBy<TImpl>()
                .Named(id)
                .Unless((k, m) => k.HasComponent(id));
        }
    }
}
