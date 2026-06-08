using GLMS.Api.DTOs.Contracts;

namespace GLMS.Api.DTOs.Clients
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public List<ContractDto> Contracts { get; set; } = new();
    }
}