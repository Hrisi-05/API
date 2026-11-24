using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Data;
using UserAPI.DTOs;
using UserAPI.Interfaces;
using UserAPI.Models;

namespace UserAPI.Services;

public class AuthService : IAuthService
{
    private readonly APIContext _context;
    private readonly IPasswordHasher<User> _hasher;
    private readonly ITokenService _tokens;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ITwoFactorService _twoFactor;

    public AuthService(APIContext context, IPasswordHasher<User> hasher, ITokenService tokens, IEmailService emailService, IConfiguration config, ITwoFactorService twoFactor)
    {
        _context = context;
        _hasher = hasher;
        _tokens = tokens;
        _emailService = emailService;
        _config = config;
        _twoFactor = twoFactor;
    }

    public async Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest registerRequest)
    {
        var email = registerRequest.Email.Trim();

        if (await _context.Users.AnyAsync(u => u.Email == email))
            return ServiceResult<UserResponse>.Fail(ServiceError.Conflict, "This email is already taken.");

        var user = new User
        {
            DisplayName = registerRequest.DisplayName.Trim(),
            Email = email
        };
        user.PasswordHash = _hasher.HashPassword(user, registerRequest.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return ServiceResult<UserResponse>.Ok(UserMapper.ToResponse(user));
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest loginRequest)
    {
        var email = loginRequest.Email.Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        
        const string error = "Invalid email or password.";

        if (user == null)
            return ServiceResult<LoginResponse>.Fail(ServiceError.Unauthorized, error);

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
        if (result == PasswordVerificationResult.Failed)
            return ServiceResult<LoginResponse>.Fail(ServiceError.Unauthorized, error);

        if (user.TwoFactorEnabled)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.TwoFactorCode))
                return ServiceResult<LoginResponse>.Fail(ServiceError.TwoFactorRequired, "2FA_REQUIRED");

            if (!_twoFactor.Verify(user, loginRequest.TwoFactorCode))
                    return ServiceResult<LoginResponse>.Fail(ServiceError.Unauthorized, "Невалиден код");
        }


            return ServiceResult<LoginResponse>.Ok(new LoginResponse
        {
            Token = _tokens.CreateToken(user),
            User = UserMapper.ToResponse(user)
        });
    }
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return; 

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.PasswordResetTokenHash = _tokens.HashToken(token);
        user.PasswordResetExpiresAt = DateTime.UtcNow.AddMinutes(30);
        await _context.SaveChangesAsync();
        await _emailService.SendAsync(email, "Reset на парола",
            $@"<p>Токен за нова парола (важи 30 минути):</p>
       <p><b>{token}</b></p>");
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null
            || user.PasswordResetTokenHash is null
            || user.PasswordResetExpiresAt is null
            || user.PasswordResetExpiresAt < DateTime.UtcNow)
            return false;

        var ok = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(_tokens.HashToken(token)),
            Encoding.UTF8.GetBytes(user.PasswordResetTokenHash));
        if (!ok) return false;

        user.PasswordHash = _hasher.HashPassword(user, newPassword);
        user.PasswordResetTokenHash = null;      
        user.PasswordResetExpiresAt = null;
        await _context.SaveChangesAsync();
        return true;
    }

}
