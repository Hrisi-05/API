using MimeKit;
using OtpNet;
using UserAPI.Data;
using UserAPI.Interfaces;
using UserAPI.Models;

namespace UserAPI.Services
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly APIContext _context;
        public TwoFactorService(APIContext context)
        {
            _context = context;
        }
        public async Task<(string Secret, string Uri)?> SetupAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null || user.TwoFactorEnabled)
                return null;
            var secret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
            user.TwoFactorSecret = secret;
            await _context.SaveChangesAsync();

            var uri = $"?secret={secret}&issuer=UserApi&digits=6";
            return (secret, uri);
        }
        public async Task<bool> EnableAsync(int userId, string code)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null || !Verify(user, code)) return false;

            user.TwoFactorEnabled = true;
            await _context.SaveChangesAsync();
            return true;

        }
        public async Task<bool> DisableAsync(int userId, string code)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null || !user.TwoFactorEnabled || !Verify(user, code)) return false;

            user.TwoFactorEnabled = false;
            user.TwoFactorSecret = null;
            await _context.SaveChangesAsync();
            return true;

        }
        public bool Verify(User user, string code)
        {
            if (user.TwoFactorSecret is null) return false;

            var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret));

            return totp.VerifyTotp(code.Replace(" ", ""), out _, VerificationWindow.RfcSpecifiedNetworkDelay);
        }

    }
}
