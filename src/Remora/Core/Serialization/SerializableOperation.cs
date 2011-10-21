using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Remora.Core.Impl;

namespace Remora.Core.Serialization
{
    [DataContract(Name="operation", Namespace = SerializationConstants.Namespace)]
    public class SerializableOperation
    {
        private static DataContractSerializer _serializer = new DataContractSerializer(typeof(SerializableOperation));

        public static SerializableOperation Deserialize(Stream stream)
        {
            return (SerializableOperation)_serializer.ReadObject(stream);
        }

        public SerializableOperation()
        {
        }

        public SerializableOperation(IRemoraOperation operation)
        {
            if(operation == null) throw new ArgumentNullException("operation");
            Contract.EndContractBlock();

            OperationId = operation.OperationId;
            IncomingUri = operation.IncomingUri != null ? operation.IncomingUri.ToString() : null;
            Kind = operation.Kind;
            OnError = operation.OnError;
            if (operation.Exception != null)
            {
                ExceptionType = operation.Exception.GetType().AssemblyQualifiedName;
                ExceptionMessage = operation.Exception.ToString();
            }

            if(operation.Request != null)
                Request = new SerializableRequest(operation.Request);

            if (operation.Response != null)
                Response = new SerializableResponse(operation.Response);

            CreatedAtUtc = operation.CreatedAtUtc;

            if (operation.ExecutingPipeline != null) { 
                PipelineName = operation.ExecutingPipeline.Id;
                if (operation.ExecutingPipeline.Definition != null)
                    PipelineComponents = string.Join(",", operation.ExecutingPipeline.Definition.ComponentDefinitions.Select(x => x.RefId));
            }
        }

        public void Serialize(Stream stream)
        {
            _serializer.WriteObject(stream, this);
        }

        [DataMember(Name = "operationId")]
        public Guid OperationId { get; set; }

        [DataMember(Name = "incomingUri")]
        public string IncomingUri { get; set; }

        [DataMember(Name = "request")]
        public SerializableRequest Request { get; set; }

        [DataMember(Name = "response")]
        public SerializableResponse Response { get; set; }

        [DataMember(Name = "kind")]
        public RemoraOperationKind Kind { get; set; }

        [DataMember(Name = "exceptionType")]
        public string ExceptionType { get; set; }

        [DataMember(Name = "exceptionMessage")]
        public string ExceptionMessage { get; set; }

        [DataMember(Name = "onError")]
        public bool OnError { get; set; }

        [DataMember(Name = "createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }

        [DataMember(Name = "elapsedMs")]
        public long ElapsedMilliseconds { get; set; }

        [DataMember(Name = "pipeline")]
        public string PipelineName { get; set; }

        [DataMember(Name = "pipelineComponents")]
        public string PipelineComponents { get; set; }
    }
}
