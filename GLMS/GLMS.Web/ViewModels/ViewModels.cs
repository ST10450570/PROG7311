using GLMS.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels
{
    public class ContractFilterViewModel
    {
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public ContractStatus? Status { get; set; }
        public IEnumerable<Contract> Results { get; set; } = new List<Contract>();
    }

    public class CreateContractViewModel
    {
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);
        public ServiceLevel ServiceLevel { get; set; }
        public IFormFile? SignedAgreement { get; set; }
    }

    public class CreateServiceRequestViewModel
    {
        [Required(ErrorMessage = "Please select an active contract to proceed.")]
        public int? ContractId { get; set; }

        [Required(ErrorMessage = "A description of the service request is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the estimated cost in USD.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "The cost must be greater than 0.")]
        public decimal? CostUsd { get; set; }

        public decimal ExchangeRate { get; set; }
        public decimal CostZar => (CostUsd ?? 0) * ExchangeRate;
    }
}
