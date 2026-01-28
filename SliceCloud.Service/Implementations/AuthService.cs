using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Service.Implementations;

public class AuthService(IUsersLoginRepository usersLoginRepository) : IAuthService
{
    private readonly IUsersLoginRepository _usersLoginRepository = usersLoginRepository;

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

    #endregion
}
