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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        private readonly IRemoraConfig _config;
        private readonly IKernel _kernel;
        private ILogger _logger = NullLogger.Instance;

        public PipelineFactory(IKernel kernel, IRemoraConfig config)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            if (config == null) throw new ArgumentNullException("config");
            Contract.EndContractBlock();

            _kernel = kernel;
            _config = config;
        }

        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IPipelineFactory Members

        public IPipeline Get(IRemoraOperation operation)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            Contract.EndContractBlock();

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Finding appropriate pipeline for {0}...", operation.IncomingUri);

            if (_config.PipelineDefinitions != null)
            {
                foreach (var pipelineDef in _config.PipelineDefinitions)
                {
                    Regex regex;
                    try
                    {
                        regex = new Regex(pipelineDef.UriFilterRegex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidConfigurationException(string.Format("There has been an error while initializing the regular expression for pipeline {0}: {1}", pipelineDef.Id, pipelineDef.UriFilterRegex), ex);
                    }

                    if (regex.IsMatch(operation.IncomingUri.ToString()))
                    {
                        if (Logger.IsDebugEnabled)
                            Logger.DebugFormat("Found pipeline definition {0} (filter: {1}) for {2}. Creating IPipeline...", pipelineDef.Id, pipelineDef.UriFilterRegex, operation.IncomingUri);

                        var components = new List<IPipelineComponent>();
                        foreach (var componentDef in pipelineDef.ComponentDefinitions)
                        {
                            try
                            {
                                components.Add(_kernel.Resolve<IPipelineComponent>(componentDef.RefId));
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidConfigurationException(string.Format("There has been an error while trying to resolve component with id {0} referenced in pipeline {1}. Maybe it is not referenced in castle or its parameters are inapproriate (check the service type, it must be IPipelineComponent).", componentDef.RefId, pipelineDef.Id), ex);
                            }
                        }

                        if (Logger.IsDebugEnabled)
                            Logger.DebugFormat("Pipeline {0} created for {1}.", pipelineDef.Id, operation.IncomingUri);

                        if (!string.IsNullOrWhiteSpace(pipelineDef.UriRewriteRegex))
                        {
                            try
                            {
                                operation.Request.Uri = new Uri(regex.Replace(operation.IncomingUri.ToString(), pipelineDef.UriRewriteRegex));
                                if (Logger.IsDebugEnabled)
                                    Logger.DebugFormat("Wrote outgoing uri for {0}: {1}.", operation.IncomingUri, operation.Request.Uri);
                            }
                            catch (Exception ex)
                            {
                                throw new UrlRewriteException(string.Format("There has been an error while rewriting uri {0} with filter {1} and rewrite {2}.", operation.IncomingUri, pipelineDef.UriFilterRegex, pipelineDef.UriRewriteRegex), ex);
                            }
                        }

                        var pipeline = new Pipeline(pipelineDef.Id, components, pipelineDef);
                        operation.ExecutingPipeline = pipeline;
                        return pipeline;
                    }
                    else
                    {
                        if (Logger.IsDebugEnabled)
                            Logger.DebugFormat("Skipped pipeline definition {0} (filter: {1}) for {2}.", pipelineDef.Id, pipelineDef.UriFilterRegex, operation.IncomingUri);
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
