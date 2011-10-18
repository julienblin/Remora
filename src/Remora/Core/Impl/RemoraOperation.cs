using System;

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

        public string IncomingContentType { get; set; }

        #region IRemoraOperation Members

        public Guid OperationId { get; private set; }

        public IRemoraRequest IncomingRequest { get; private set; }

        public IRemoraRequest Request { get; private set; }

        public IRemoraResponse Response { get; private set; }

        public RemoraOperationKind Kind { get; set; }

        public Exception Exception { get; set; }

        public bool OnError { get { return Exception != null; } }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}({1})", OperationId, IncomingRequest.Uri);
        }
    }
}
