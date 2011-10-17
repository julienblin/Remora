using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Core.Impl
{
    public class RemoraOperation : IRemoraOperation
    {
        public RemoraOperation()
        {
            OperationId = Guid.NewGuid();
            IncomingRequest = new RemoraRequest();
            Request = new RemoraRequest();
            Response = new RemoraResponse();
        }

        public Guid OperationId { get; private set; }

        public string IncomingContentType { get; set; }

        public IRemoraRequest IncomingRequest { get; private set; }

        public IRemoraRequest Request { get; private set; }

        public IRemoraResponse Response { get; private set; }

        public RemoraOperationKind Kind { get; set; }

        public Exception Exception { get; set; }

        public bool OnError { get { return Exception != null; } }

        public override string ToString()
        {
            return string.Format("{0}({1})", OperationId, IncomingRequest.Uri);
        }
    }
}
