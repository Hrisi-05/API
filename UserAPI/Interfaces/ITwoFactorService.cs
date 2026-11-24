using UserAPI.Models;

namespace UserAPI.Interfaces
{
    public interface ITwoFactorService
    {
        Task<(string Secret, string Uri)?> SetupAsync(int userId);
        Task<bool> EnableAsync(int userId, string code);
        Task<bool> DisableAsync(int userId, string code);
        bool Verify(User user, string code);
    }
}
