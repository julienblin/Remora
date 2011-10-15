
namespace Remora.Core
{
    /// <summary>
    /// Represents a Remora operation
    /// </summary>
    public interface IRemoraOperation
    {
        /// <summary>
        /// Request that is coming from the original system to Remora
        /// </summary>
        IRemoraRequest IncomingRequest { get; }

        /// <summary>
        /// Request that is emitted from Remora to the final destination system
        /// </summary>
        IRemoraRequest OutgoingRequest { get; }

        /// <summary>
        /// Response that is coming back from the destination system to Remora
        /// </summary>
        IRemoraResponse IncomingResponse { get; }

        /// <summary>
        /// Response returned to the original system by Remora
        /// </summary>
        IRemoraResponse OutgoingResponse { get; }
    }
}
