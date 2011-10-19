using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Remora.Pipeline;

namespace Remora.Core.Impl
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
                ExceptionMessage = operation.Exception.Message;
            }

            if(operation.Request != null)
                Request = new SerializableRequest(operation.Request);

            if (operation.Response != null)
                Response = new SerializableResponse(operation.Response);
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

        [DataContract(Name = "request", Namespace = SerializationConstants.Namespace)]
        public class SerializableRequest
        {
            public SerializableRequest()
            {
            }

            public SerializableRequest(IRemoraRequest request)
            {
                if (request == null) throw new ArgumentNullException("request");
                Contract.EndContractBlock();

                Headers = request.HttpHeaders.Select(k => new SerializableHeader(k.Key, k.Value)).ToArray();
                ContentEncoding = request.ContentEncoding != null ? request.ContentEncoding.HeaderName : null;
                Content = Encoding.UTF8.GetString(request.Data);
                Method = request.Method;
                Uri = request.Uri != null ? request.Uri.ToString() : null;
            }

            public byte[] GetData()
            {
                return Encoding.GetEncoding(ContentEncoding).GetBytes(Content);
            }

            [DataMember(Name = "contentEncoding")]
            public string ContentEncoding { get; set; }

            [DataMember(Name = "content")]
            public string Content { get; set; }

            [DataMember(Name = "headers")]
            public SerializableHeader[] Headers { get; set; }

            [DataMember(Name = "method")]
            public string Method { get; set; }

            [DataMember(Name = "uri")]
            public string Uri { get; set; }
        }

        [DataContract(Name = "response", Namespace = SerializationConstants.Namespace)]
        public class SerializableResponse
        {
            public SerializableResponse()
            {
            }

            public SerializableResponse(IRemoraResponse response)
            {
                if (response == null) throw new ArgumentNullException("response");
                Contract.EndContractBlock();

                Headers = response.HttpHeaders.Select(k => new SerializableHeader(k.Key, k.Value)).ToArray();
                ContentEncoding = response.ContentEncoding != null ? response.ContentEncoding.HeaderName : null;
                Content = Encoding.UTF8.GetString(response.Data);
                StatusCode = response.StatusCode;
                Uri = response.Uri != null ? response.Uri.ToString() : null;
            }

            public byte[] GetData()
            {
                return Encoding.GetEncoding(ContentEncoding).GetBytes(Content);
            }

            [DataMember(Name = "contentEncoding")]
            public string ContentEncoding { get; set; }

            [DataMember(Name = "content")]
            public string Content { get; set; }

            [DataMember(Name = "headers")]
            public SerializableHeader[] Headers { get; set; }

            [DataMember(Name = "statusCode")]
            public int StatusCode { get; set; }

            [DataMember(Name = "uri")]
            public string Uri { get; set; }
        }

        [DataContract(Name = "header", Namespace = SerializationConstants.Namespace)]
        public class SerializableHeader
        {
            public SerializableHeader()
            {
            }

            public SerializableHeader(string name, string value)
            {
                Name = name;
                Value = value;
            }

            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "value")]
            public string Value { get; set; }
        }
    }
}
