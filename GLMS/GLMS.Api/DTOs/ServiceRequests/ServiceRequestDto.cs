namespace GLMS.Api.DTOs.ServiceRequests
{
    public class ServiceRequestDto
    {

        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostUsd { get; set; }
        public decimal CostZar { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? ClientName { get; set; }
    }
}
