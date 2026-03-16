using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Service.Implementations;

public class AuthService(IUsersLoginRepository usersLoginRepository, IUsersRepository usersRepository) : IAuthService
{
    private readonly IUsersLoginRepository _usersLoginRepository = usersLoginRepository;
    private readonly IUsersRepository _usersRepository = usersRepository;

    #region AuthenticateUser

    public async Task<UsersLogin?> AuthenticateUserAsync(string userEmail, string userPassword)
    {
        string hashedPassword = PasswordUtils.HashPassword(userPassword);

        UsersLogin? usersLogin = await _usersLoginRepository.GetUsersLoginWithUserAsQueryable().FirstOrDefaultAsync(
            u => u.Email == userEmail
            && u.PasswordHash == hashedPassword
            && u.IsFirstLogin == false
            && u.User!.IsDeleted == false
            && u.User.Status == (int)UserStatus.Active
            );

        if (usersLogin == null) return null;

        return usersLogin;
    }

    #endregion

    #region GetUserLoginByEmail

    public async Task<UsersLogin?> GetUserLoginByEmailAsync(string userEmail)
    {
        return await _usersLoginRepository.GetUsersLoginAsQueryable()
                                            .FirstOrDefaultAsync(u => u.Email!.ToLower() == userEmail.ToLower()) ?? null;

    }

    #endregion

    #region GeneratePasswordResetToken

    public async Task<string> GeneratePasswordResetTokenAsync(string userEmail)
    {
        UsersLogin? usersLogin = await _usersLoginRepository.GetUsersLoginAsQueryable()
                               .FirstOrDefaultAsync(u => u.Email!.ToLower() == userEmail.ToLower())
                                ?? throw new InvalidOperationException("User not found with the provided email.");

        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        usersLogin.ResetToken = token;
        usersLogin.ResetTokenExpiration = DateTime.UtcNow.AddHours(24);
        usersLogin.IsResetTokenUsed = false;

        await _usersLoginRepository.UpdateUsersLoginAsync(usersLogin);

        return token;
    }

    #endregion

    #region ValidatePasswordResetToken

    public async Task<bool> ValidatePasswordResetTokenAsync(string token)
    {
        UsersLogin? usersLogin = await _usersLoginRepository.GetUsersLoginAsQueryable().FirstOrDefaultAsync(u => u.ResetToken == token);

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
        UsersLogin? usersLogin = await _usersLoginRepository.GetUsersLoginAsQueryable().FirstOrDefaultAsync(u => u.ResetToken == token);


        if (usersLogin == null || usersLogin.ResetTokenExpiration.GetValueOrDefault() < DateTime.UtcNow || usersLogin.IsResetTokenUsed == true)
        {
            return false;
        }

        string hashedPassword = PasswordUtils.HashPassword(newPassword);

        usersLogin.PasswordHash = hashedPassword;
        usersLogin.IsResetTokenUsed = true;
        usersLogin.IsFirstLogin = false;

        User? user = await _usersRepository.GetUserByIdAsync(usersLogin.UserId ?? 0);

        if (user is not null)
        {
            user.PasswordHash = hashedPassword;
            await _usersRepository.UpdateUserAsync(user);
        }

        return await _usersLoginRepository.UpdateUsersLoginAsync(usersLogin) > 0;
    }

    #endregion
}
