using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels.ServiceRequests
{
    public class CreateServiceRequestViewModel
    {
        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Cost (USD)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero.")]
        public decimal CostUsd { get; set; }

        [Display(Name = "Live Exchange Rate (USD to ZAR)")]
        public decimal ExchangeRate { get; set; }

        [Display(Name = "Estimated Cost (ZAR)")]
        public decimal CostZar { get; set; }
    }
}