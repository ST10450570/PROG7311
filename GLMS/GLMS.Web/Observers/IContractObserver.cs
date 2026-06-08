namespace GLMS.Web.Observers
{
    public interface IContractObserver
    {
        void OnStatusChanged(int contractId, string newStatus);
    }
}
