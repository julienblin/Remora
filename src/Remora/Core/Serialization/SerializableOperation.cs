#region Licence

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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Remora.Core.Serialization
{
    [DataContract(Name = "operation", Namespace = SerializationConstants.Namespace)]
    public class SerializableOperation
    {
        private static readonly DataContractSerializer _serializer =
            new DataContractSerializer(typeof (SerializableOperation));

        public SerializableOperation()
        {
        }

        public SerializableOperation(IRemoraOperation operation)
        {
            if (operation == null) throw new ArgumentNullException("operation");
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

            if (operation.Request != null)
                Request = new SerializableRequest(operation.Request);

            if (operation.Response != null)
                Response = new SerializableResponse(operation.Response);

            CreatedAtUtc = operation.CreatedAtUtc;

            if (operation.ExecutingPipeline != null)
            {
                PipelineName = operation.ExecutingPipeline.Id;
                if (operation.ExecutingPipeline.Definition != null)
                    PipelineComponents = string.Join(",",
                                                     operation.ExecutingPipeline.Definition.ComponentDefinitions.Select(
                                                         x => x.RefId));
            }
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

        public static SerializableOperation Deserialize(Stream stream)
        {
            return (SerializableOperation) _serializer.ReadObject(stream);
        }

        public void Serialize(Stream stream)
        {
            _serializer.WriteObject(stream, this);
        }
    }
}