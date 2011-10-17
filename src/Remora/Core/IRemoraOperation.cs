
using System;

namespace Remora.Core
{
    /// <summary>
    /// Represents a Remora operation
    /// </summary>
    public interface IRemoraOperation
    {
        Guid OperationId { get; }

        IRemoraRequest IncomingRequest { get; }

        IRemoraRequest Request { get; }

        IRemoraResponse Response { get; }

        RemoraOperationKind Kind { get; set; }

        Exception Exception { get; set; }

        bool OnError { get; }
    }
}
