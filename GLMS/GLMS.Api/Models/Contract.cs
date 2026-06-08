namespace GLMS.Api.Models
{
    public enum ContractStatus
    {
        Draft = 0,
        Active = 1,
        Expired =2,
        OnHold =3
    }

    public enum ServiceLevel
    {
        Standard =0,
        Premium =1
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
