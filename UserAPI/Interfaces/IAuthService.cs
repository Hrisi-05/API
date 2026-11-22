using UserAPI.DTOs;

namespace UserAPI.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    }
}

