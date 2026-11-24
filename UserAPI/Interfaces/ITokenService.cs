using UserAPI.Models;

namespace UserAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        string HashToken(string token);
    }
}
