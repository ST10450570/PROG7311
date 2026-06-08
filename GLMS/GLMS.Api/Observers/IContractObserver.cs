namespace GLMS.Api.Observers
{
    public interface IContractObserver
    {
        void OnStatusChanged(int contractId, string newStatus);
    }
}
