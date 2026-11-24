using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Services;

public static class UserMapper
{
    public static UserResponse ToResponse(User user) => new()
    {
        UserId = user.UserId,
        DisplayName = user.DisplayName,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    };
}
