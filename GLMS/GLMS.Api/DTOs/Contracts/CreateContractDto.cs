using GLMS.Api.Models;


namespace GLMS.Api.DTOs.Contracts
{
    public class CreateContractDto
    {
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public ServiceLevel ServiceLevel { get; set; }
    }
}