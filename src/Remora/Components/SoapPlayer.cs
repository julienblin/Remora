﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Core.Serialization;
using Remora.Exceptions;
using Remora.Pipeline;
using Remora.Transformers;
using Remora.Extensions;

namespace Remora.Components
{
    public class SoapPlayer : AbstractPipelineComponent
    {
        private readonly ISoapTransformer _soapTransformer;
        public const string ComponentId = @"soapPlayer";

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public SoapPlayer(ISoapTransformer soapTransformer)
        {
            if (soapTransformer == null) throw new ArgumentNullException("soapTransformer");
            Contract.EndContractBlock();

            _soapTransformer = soapTransformer;
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (componentDefinition == null) throw new ArgumentNullException("componentDefinition");
            if (callback == null) throw new ArgumentNullException("callback");
            Contract.EndContractBlock();

            switch (operation.Kind)
            {
                case RemoraOperationKind.Soap:
                    PlaySoapOperation(operation, componentDefinition);
                    break;
                default:
                    throw new SoapPlayerException(string.Format("Unable to playback operation {0} because it is not a soap operation.", operation));
            }
            callback(false);
        }

        private void PlaySoapOperation(IRemoraOperation operation, IComponentDefinition componentDefinition)
        {
            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Trying to reply with a mock for {0}...", operation);

            if (!componentDefinition.Properties.ContainsKey("directory"))
            {
                throw new SoapPlayerException("Unable to playback because no directory has been provided. Please specify a source directory in the component definition.");
            }
            var directoryPath = componentDefinition.Properties["directory"];

            if (!operation.Request.HttpHeaders.ContainsKey("SOAPAction"))
            {
                throw new SoapPlayerException(string.Format("Unable to playback operation {0} because it doesn't have a SOAPAction header.", operation));
            }

            var soapAction = operation.Request.HttpHeaders["SOAPAction"];
            IEnumerable<string> candidateFiles;
            try
            {
                candidateFiles = Directory.EnumerateFiles(directoryPath, soapAction.MakeValidFileName() + ".*");
            }
            catch (Exception ex)
            {
                throw new SoapPlayerException(string.Format("There has been an error while enumerating files in {0}", directoryPath), ex);
            }

            var requestDoc = _soapTransformer.LoadSoapDocument(operation.Request).Normalize();

            foreach (var candidateFile in candidateFiles)
            {
                try
                {
                    if(Logger.IsDebugEnabled)
                        Logger.DebugFormat("Opening file {0}...", candidateFile);

                    SerializableOperation serializableOperation;
                    using (var readStream = File.OpenRead(candidateFile))
                    {
                        serializableOperation = SerializableOperation.Deserialize(readStream);
                        if (MockMatch(serializableOperation, requestDoc, componentDefinition))
                        {
                            if(Logger.IsInfoEnabled)
                                Logger.InfoFormat("Found appropriate mock for {0}: {1}.", operation, candidateFile);

                            operation.Response.ContentEncoding = Encoding.GetEncoding(serializableOperation.Response.ContentEncoding);
                            foreach (var header in serializableOperation.Response.Headers)
                            {
                                operation.Response.HttpHeaders.Add(header.Name, header.Value);
                            }
                            operation.Response.StatusCode = serializableOperation.Response.StatusCode;
                            operation.Response.Data = serializableOperation.Response.GetData();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new SoapPlayerException(string.Format("Error while opening mock file {0}.", candidateFile), ex);
                }
            }
            throw new SoapPlayerException(string.Format("Unable to find appropriate mock for operation {0} in directory {1}.", operation, directoryPath));
        }

        private bool MockMatch(SerializableOperation serializableOperation, XDocument requestDoc, IComponentDefinition componentDefinition)
        {
            var refDoc = XDocument.Parse(serializableOperation.Request.Content).Normalize();

            var refBody = _soapTransformer.GetBody(refDoc);
            var currentBody = _soapTransformer.GetBody(requestDoc);

            return XNode.DeepEquals(refBody, currentBody);
        }
    }
}