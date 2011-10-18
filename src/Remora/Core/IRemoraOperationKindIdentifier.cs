namespace Remora.Core
{
    public interface IRemoraOperationKindIdentifier
    {
        RemoraOperationKind Identify(IRemoraOperation operation);
    }
}
