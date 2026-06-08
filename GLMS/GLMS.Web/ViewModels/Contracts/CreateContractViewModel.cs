using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels.Contracts
{
    public class CreateContractViewModel
    {
        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        [Display(Name = "Service Level")]
        public ServiceLevel ServiceLevel { get; set; }

        [Display(Name = "Upload Signed Agreement (PDF)")]
        public IFormFile? SignedAgreement { get; set; }
    }
}