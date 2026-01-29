using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Service.Implementations;

public class AuthService(IUsersLoginRepository usersLoginRepository, IConfiguration configuration) : IAuthService
{
    private readonly IUsersLoginRepository _usersLoginRepository = usersLoginRepository;
    private readonly IConfiguration _configuration = configuration;

    #region AuthenticateUser

    public async Task<UsersLogin?> AuthenticateUserAsync(string userEmail, string userPassword)
    {
        string hashedPassword = PasswordUtils.HashPassword(userPassword);
        UsersLogin? usersLogin = await _usersLoginRepository.GetUserLoginAsync(userEmail, hashedPassword);

        if (usersLogin == null) return null;

        return usersLogin;
    }

    #endregion

    #region GetUserLoginByEmail

    public async Task<UsersLogin?> GetUserLoginByEmailAsync(string userEmail)
    {
        UsersLogin? usersLogin = await _usersLoginRepository.GetUserLoginByEmailAsync(userEmail);
        if (usersLogin is null)
            return null;
        return usersLogin;
    }

    #endregion

    #region GeneratePasswordResetToken

    public async Task<string> GeneratePasswordResetTokenAsync(string userEmail)
    {
        UsersLogin? usersLogin = await _usersLoginRepository.GetUserLoginByEmailAsync(userEmail) ?? throw new InvalidOperationException("User not found with the provided email.");

        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        await _usersLoginRepository.SavePasswordResetTokenAsync(usersLogin.UserLoginId,
          token,
          DateTime.UtcNow.AddHours(24),
          false);

        return token;
    }

    #endregion

    #region ValidatePasswordResetToken

    public async Task<bool> ValidatePasswordResetTokenAsync(string token)
    {
        UsersLogin? usersLogin = await _usersLoginRepository.GetUserByResetTokenAsync(token);
        if (usersLogin == null || usersLogin.ResetTokenExpiration.GetValueOrDefault() < DateTime.UtcNow || usersLogin.IsResetTokenUsed == true)
        {
            return false;
        }
        return true;
    }

    #endregion

    #region UpdateUserPassword

    public async Task<bool> UpdateUserPasswordAsync(string token, string newPassword)
    {
        UsersLogin? usersLogin = await _usersLoginRepository.GetUserByResetTokenAsync(token);

        if (usersLogin == null || usersLogin.ResetTokenExpiration.GetValueOrDefault() < DateTime.UtcNow || usersLogin.IsResetTokenUsed == true)
        {
            return false;
        }

        string hashedPassword = PasswordUtils.HashPassword(newPassword);
        bool passwordUpdated = await _usersLoginRepository.SetUserPasswordAsync(usersLogin.UserLoginId, hashedPassword);
        if (!passwordUpdated)
        {
            return false;
        }

        usersLogin.IsResetTokenUsed = true;
        usersLogin.IsFirstLogin = false;

        bool isResetTokenInvalidated = await _usersLoginRepository.InvalidateResetTokenAsync(usersLogin.UserLoginId);
        if (!isResetTokenInvalidated)
        {
            return false;
        }

        return true;
    }

    public UserCredentialViewModel DecodeJwtToken(string token)
    {
        JwtSecurityTokenHandler? handler = new();
        byte[]? key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

        TokenValidationParameters validations = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        try
        {
            ClaimsPrincipal principal = handler.ValidateToken(token, validations, out _);

            string? userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;

            _ = int.TryParse(userIdClaim, out int userId);

            return new UserCredentialViewModel
            {
                Id = userId,
                Email = emailClaim ?? string.Empty
            };
        }
        catch
        {
            return new UserCredentialViewModel
            {
                Id = 0,
                Email = string.Empty
            };
        }
    }


    #endregion
}
