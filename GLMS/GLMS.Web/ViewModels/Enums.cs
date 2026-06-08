namespace GLMS.Web.ViewModels
{
    public enum ContractStatus
    {
        Draft = 0,
        Active = 1,
        Expired = 2,
        OnHold = 3
    }

    public enum ServiceLevel
    {
        Standard = 0,
        Premium = 1
    }

    public enum ServiceRequestStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
}