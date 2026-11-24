using System.ComponentModel.DataAnnotations;

namespace UserAPI.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required]
        public string Email { get; set; } = "";
    }
}
