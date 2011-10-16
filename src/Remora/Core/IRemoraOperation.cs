
using System;

namespace Remora.Core
{
    /// <summary>
    /// Represents a Remora operation
    /// </summary>
    public interface IRemoraOperation
    {
        Guid OperationId { get; }

        Uri IncomingUri { get; set; }

        IRemoraRequest Request { get; }

        IRemoraResponse Response { get; }

        Exception Exception { get; set; }

        bool OnError { get; }
    }
}
