using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels.Clients
{
    public class CreateClientViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required, Phone]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;
    }
}