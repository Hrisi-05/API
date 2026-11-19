namespace UserAPI.DTOs
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
