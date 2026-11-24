using UserAPI.DTOs;

namespace UserAPI.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
        Task ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}

