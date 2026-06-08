namespace GLMS.Api.DTOs.Clients
{
    public class CreateClientDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }
}