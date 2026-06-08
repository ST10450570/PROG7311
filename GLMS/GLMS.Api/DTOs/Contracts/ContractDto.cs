using GLMS.Api.DTOs.ServiceRequests;
using GLMS.Api.Models;

namespace GLMS.Api.DTOs.Contracts
{
    public class ContractDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceLevel { get; set; } = string.Empty;
        public string? SignedAgreementPath { get; set; }
        public List<ServiceRequestDto> ServiceRequests { get; set; } = new();
    }
}