using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Remora.Configuration;
using Remora.Core;
using Remora.Exceptions;

namespace Remora.Pipeline.Impl
{
    public class PipelineFactory : IPipelineFactory
    {
        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        private readonly IKernel _kernel;
        private IEnumerable<PipelineConfiguration> _pipelineConfigs;

        public PipelineFactory(IKernel kernel)
            : this(kernel, PipelinesConfigurationSectionHandler.GetConfiguration())
        {
        }

        public PipelineFactory(IKernel kernel, IEnumerable<PipelineConfiguration> pipelineConfigs)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            if (pipelineConfigs == null) throw new ArgumentNullException("pipelineConfigs");
            Contract.EndContractBlock();

            _kernel = kernel;
            _pipelineConfigs = pipelineConfigs;
        }

        public IPipeline Get(IRemoraOperation operation)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            Contract.EndContractBlock();

            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Finding appropriate pipeline for {0}...", operation.IncomingRequest.Uri);

            foreach (var pipelineConfig in _pipelineConfigs)
            {
                Regex regex;
                try
                {
                    regex = new Regex(pipelineConfig.UriFilterRegex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                }
                catch (Exception ex)
                {
                    throw new InvalidConfigurationException(string.Format("There has been an error while initializing the regular expression for pipeline {0}: {1}", pipelineConfig.Id, pipelineConfig.UriFilterRegex), ex);
                }

                if(regex.IsMatch(operation.IncomingRequest.Uri))
                {
                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("Found pipeline configuration {0} (filter: {1}) for {2}. Creating IPipeline...", pipelineConfig.Id, pipelineConfig.UriFilterRegex, operation.IncomingRequest.Uri);

                    var components = new List<IPipelineComponent>();
                    foreach (var componentId in pipelineConfig.Components)
                    {
                        try
                        {
                            components.Add(_kernel.Resolve<IPipelineComponent>(componentId));
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidConfigurationException(string.Format("There has been an error while trying to resolve component with id {0} referenced in pipeline {1}. Maybe it is not referenced in castle or its parameters are inapproriate (check the service type, it must be IPipelineComponent). Please check inner exception for details.", componentId, pipelineConfig.Id), ex);
                        }
                    }

                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("Pipeline {0} created for {1}.", pipelineConfig.Id, operation.IncomingRequest.Uri);

                    if(!string.IsNullOrWhiteSpace(pipelineConfig.UriRewriteRegex))
                    {
                        try
                        {
                            operation.OutgoingRequest.Uri = regex.Replace(operation.IncomingRequest.Uri, pipelineConfig.UriRewriteRegex);
                            if (Logger.IsDebugEnabled)
                                Logger.DebugFormat("Wrote outgoing uri for {0}: {1}.", operation.IncomingRequest.Uri, operation.OutgoingRequest.Uri);
                        }
                        catch (Exception ex)
                        {
                            throw new UrlRewriteException(string.Format("There has been an error while rewriting uri {0} with filter {1} and rewrite {2}. Check inner exception for details.", operation.IncomingRequest.Uri, pipelineConfig.UriFilterRegex, pipelineConfig.UriRewriteRegex), ex);
                        }
                    }

                    return new Pipeline(pipelineConfig.Id, components);
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("Skipped pipeline configuration {0} (filter: {1}) for {2}.", pipelineConfig.Id, pipelineConfig.UriFilterRegex, operation.IncomingRequest.Uri);
                }
            }

            return null;
        }
    }
}
