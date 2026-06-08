namespace GLMS.Web.Models
{
    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        OnHold
    }

    public enum ServiceLevel
    {
        Standard,
        Premium
    }

    public class Contract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Draft;
        public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Standard;

        public string? SignedAgreementPath { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
