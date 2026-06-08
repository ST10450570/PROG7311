using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}