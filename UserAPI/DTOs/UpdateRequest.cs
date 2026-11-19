using System.ComponentModel.DataAnnotations;

namespace UserAPI.DTOs
{
    public class UpdateRequest
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
    }
}
