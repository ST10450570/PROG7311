namespace GLMS.Api.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
