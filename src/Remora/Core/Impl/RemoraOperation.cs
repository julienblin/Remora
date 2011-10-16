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
            Request = new RemoraRequest();
            Response = new RemoraResponse();
        }

        public Guid OperationId { get; private set; }

        public Uri IncomingUri { get; set; }

        public IRemoraRequest Request { get; private set; }

        public IRemoraResponse Response { get; private set; }

        public Exception Exception { get; set; }

        public bool OnError { get { return Exception != null; } }

        public override string ToString()
        {
            return string.Format("{0}({1})", OperationId, IncomingUri);
        }
    }
}
