namespace GLMS.Api.DTOs.ServiceRequests
{
    public class CreateServiceRequestDto
    {

        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostUsd { get; set; }

    }
}
