using UserAPI.DTOs;

namespace UserAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAllAsync();
        Task<ServiceResult<UserResponse>> GetByIdAsync(int id);
        Task<ServiceResult<UserResponse>> UpdateAsync(int id, UpdateRequest request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
