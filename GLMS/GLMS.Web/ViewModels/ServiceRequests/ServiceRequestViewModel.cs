using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels.ServiceRequests
{
    public class ServiceRequestViewModel
    {
        public int Id { get; set; }
        public int ContractId { get; set; }

        [Display(Name = "Client Name")]
        public string? ClientName { get; set; }

        public string Description { get; set; } = string.Empty;

        [Display(Name = "Cost (USD)")]
        public decimal CostUsd { get; set; }

        [Display(Name = "Cost (ZAR)")]
        public decimal CostZar { get; set; }

        public string Status { get; set; } = string.Empty;

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }
}