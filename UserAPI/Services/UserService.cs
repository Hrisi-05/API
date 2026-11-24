using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.DTOs;
using UserAPI.Interfaces;


namespace UserAPI.Services
{
    public class UserService : IUserService
    {
        private readonly APIContext _context;

        public UserService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<UserResponse>> GetAllAsync()
        {
            var users = await _context.Users.AsNoTracking().OrderBy(u => u.UserId).ToListAsync();
            return users.Select(UserMapper.ToResponse).ToList();
        }

        public async Task<ServiceResult<UserResponse>> GetByIdAsync(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return ServiceResult<UserResponse>.Fail(ServiceError.NotFound, "User not found.");

            return ServiceResult<UserResponse>.Ok(UserMapper.ToResponse(user));
        }

        public async Task<ServiceResult<UserResponse>> UpdateAsync(int id, UpdateRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return ServiceResult<UserResponse>.Fail(ServiceError.NotFound, "User not found.");

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var email = request.Email.Trim().ToLowerInvariant();

                if (email != user.Email && await _context.Users.AnyAsync(u => u.Email == email))
                    return ServiceResult<UserResponse>.Fail(ServiceError.Conflict, "This email is already taken.");

                user.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(request.DisplayName))
                user.DisplayName = request.DisplayName.Trim();

            await _context.SaveChangesAsync();
            return ServiceResult<UserResponse>.Ok(UserMapper.ToResponse(user));
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return ServiceResult<bool>.Fail(ServiceError.NotFound, "User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
    }
}
