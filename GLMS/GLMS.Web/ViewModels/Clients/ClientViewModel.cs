using GLMS.Web.ViewModels.Contracts;

namespace GLMS.Web.ViewModels.Clients
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;

        public List<ContractViewModel> Contracts { get; set; } = new();
    }
}