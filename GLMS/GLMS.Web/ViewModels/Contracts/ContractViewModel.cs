using GLMS.Web.ViewModels.ServiceRequests;
using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels.Contracts
{
    public class ContractViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; } = string.Empty;

        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;

        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        public string? SignedAgreementPath { get; set; }

        public List<ServiceRequestViewModel> ServiceRequests { get; set; } = new();
    }
}