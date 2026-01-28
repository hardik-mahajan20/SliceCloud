using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class UsersLoginRepository(SliceCloudContext sliceCloudContext) : IUsersLoginRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetUserLogin

    public async Task<UsersLogin?> GetUserLoginAsync(string userEmail, string userHashedPassword)
    {
        UsersLogin? usersLogin = await _sliceCloudContext.UsersLogins.Include(u => u.User).FirstOrDefaultAsync(
            u => u.Email == userEmail
            && u.PasswordHash == userHashedPassword
            && u.IsFirstLogin == false
            && u.User!.IsDeleted == false
            && u.User.Status == 1
            );
        return usersLogin;
    }

    #endregion

    #region GetUserLoginByEmail

    public async Task<UsersLogin?> GetUserLoginByEmailAsync(string userEmail)
    {
        return await _sliceCloudContext.UsersLogins.FirstOrDefaultAsync(u => u.Email!.ToLower() == userEmail.ToLower());
    }

    #endregion

    #region SavePasswordResetToken

    public async Task SavePasswordResetTokenAsync(int userId, string passwordResetToken, DateTime expiration, bool isUsed)
    {
        UsersLogin? usersLogin = await _sliceCloudContext.UsersLogins.FindAsync(userId);
        if (usersLogin != null)
        {
            usersLogin.ResetToken = passwordResetToken;
            usersLogin.ResetTokenExpiration = expiration;
            usersLogin.IsResetTokenUsed = isUsed;
            await _sliceCloudContext.SaveChangesAsync();
        }
    }

    #endregion

    #region GetUserByResetToken

    public async Task<UsersLogin?> GetUserByResetTokenAsync(string resetToken)
    {
        return await _sliceCloudContext.UsersLogins.FirstOrDefaultAsync(u => u.ResetToken == resetToken);
    }

    #endregion

    #region SetUserPassword

    public async Task<bool> SetUserPasswordAsync(int userLoginId, string newPassword)
    {
        UsersLogin? usersLogin = await _sliceCloudContext.UsersLogins.FindAsync(userLoginId);
        if (usersLogin == null)
        {
            return false;
        }

        User? userTable = await _sliceCloudContext.Users.FindAsync(usersLogin.UserId);
        if (userTable == null)
        {
            return false;
        }

        usersLogin.PasswordHash = newPassword;
        userTable.PasswordHash = newPassword;

        _sliceCloudContext.UsersLogins.Update(usersLogin);
        _sliceCloudContext.Users.Update(userTable);
        await _sliceCloudContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region InvalidateResetToken

    public async Task<bool> InvalidateResetTokenAsync(int userLoginId)
    {
        UsersLogin? usersLogin = await _sliceCloudContext.UsersLogins
       .FirstOrDefaultAsync(u => u.UserLoginId == userLoginId);

        if (usersLogin == null)
            return false;

        usersLogin.IsResetTokenUsed = true;
        await _sliceCloudContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region CreateUserLogin

    public async Task CreateUserLoginAsync(UsersLogin usersLogin)
    {
        await _sliceCloudContext.UsersLogins.AddAsync(usersLogin);
        await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion
}
