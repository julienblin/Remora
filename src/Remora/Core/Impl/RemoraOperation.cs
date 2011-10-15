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
            IncomingRequest = new RemoraRequest();
            OutgoingRequest = new RemoraRequest();
            IncomingResponse = new RemoraResponse();
            OutgoingResponse = new RemoraResponse();
        }

        public IRemoraRequest IncomingRequest { get; private set; }

        public IRemoraRequest OutgoingRequest { get; private set; }

        public IRemoraResponse IncomingResponse { get; private set; }

        public IRemoraResponse OutgoingResponse { get; private set; }

        public override string ToString()
        {
            return string.Format("From: {0} - To: {1}", IncomingRequest.Uri ?? "<unknown>", OutgoingRequest.Uri ?? "<unknown>");
        }
    }
}
