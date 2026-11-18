using System.ComponentModel.DataAnnotations;

namespace UserAPI.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string? DisplayName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
