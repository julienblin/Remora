using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Pipeline;
using Remora.Transformers;

namespace Remora.Components
{
    public class Player : AbstractPipelineComponent
    {
        public const string ComponentId = @"player";

        private readonly ISoapTransformer _soapTransformer;
        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public Player(ISoapTransformer soapTransformer)
        {
            if (soapTransformer == null) throw new ArgumentNullException("soapTransformer");
            Contract.EndContractBlock();

            _soapTransformer = soapTransformer;
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            switch (operation.Kind)
            {
                case RemoraOperationKind.Soap:
                    PlaySoapOperation(operation, componentDefinition);
                    break;
            }
            callback(false);
        }

        private void PlaySoapOperation(IRemoraOperation operation, IComponentDefinition componentDefinition)
        {
            if (!componentDefinition.Properties.ContainsKey("directory"))
            {
            }
            var directoryPath = componentDefinition.Properties["directory"];

            var soapDoc = _soapTransformer.LoadSoapDocument(operation.Request);
            if (soapDoc == null)
            {
            }

            var soapAction = _soapTransformer.GetSoapActionName(soapDoc);
            if (soapAction == null)
            {
                
            }

            var fileName = Path.Combine(directoryPath, string.Format("{0}.xml", soapAction));

            RecordAction recordAction;
            using (var readStream = File.OpenRead(fileName))
            {
                var serializer = new DataContractSerializer(typeof (RecordAction));
                recordAction = (RecordAction) serializer.ReadObject(readStream);
            }

            operation.Response.ContentEncoding = Encoding.GetEncoding(recordAction.ResponseContentEncoding);
            foreach (var recordActionHeader in recordAction.Headers)
            {
                operation.Response.HttpHeaders.Add(recordActionHeader.Name, recordActionHeader.Value);
            }
            operation.Response.StatusCode = recordAction.ResponseStatusCode;
            operation.Response.Data = operation.Response.ContentEncoding.GetBytes(recordAction.Response);
        }
    }
}
