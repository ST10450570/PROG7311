namespace GLMS.Api.Models
{
    public enum ServiceRequestStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
    }

    public class ServiceRequest
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }

        public string Description { get; set; } = string.Empty;
        public decimal CostUsd { get; set; }
        public decimal CostZar { get; set; }
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
