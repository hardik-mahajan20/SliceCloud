using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IUsersLoginRepository
{
    /// <summary>
    /// Retrieves user's login details by their email & hashed password asynchronously.
    /// </summary>
    /// <param name="userEmail">The email of the user to retrieve.</param>
    /// <param name="userHashedPassword">The password of the user to retrieve.</param>
    /// <returns>A task that returns the user login details if found, otherwise null.</returns>
    Task<UsersLogin?> GetUserLoginAsync(string userEmail, string userHashedPassword);

    /// <summary>
    /// Retrieves a user's login details by their email asynchronously.
    /// </summary>
    /// <param name="userEmail">The email of the user to retrieve.</param>
    /// <returns>A task that returns the users's login details if found, otherwise null.</returns>
    Task<UsersLogin?> GetUserLoginByEmailAsync(string userEmail);

    /// <summary>
    /// Saves a password reset token for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="token">The reset token to save.</param>
    /// <param name="expiration">The expiration date of the token.</param>
    /// <param name="used">Indicates whether the token has been used.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SavePasswordResetTokenAsync(
        int userId,
        string passwordResetToken,
        DateTime expiration,
        bool isUsed
    );

    /// <summary>
    /// Retrieves a user's login details by their resetToken asynchronously.
    /// </summary>
    /// <param name="resetToken">The email of the user to retrieve.</param>
    /// <returns>A task that returns the user's login details if found, otherwise null.</returns>
    Task<UsersLogin?> GetUserByResetTokenAsync(string resetToken);

    /// <summary>
    /// Sets a new password for a user.
    /// </summary>
    /// <param name="userLoginId">The ID of the user's.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <returns>A task that returns true if the password was updated successfully, otherwise false.</returns>
    Task<bool> SetUserPasswordAsync(int userLoginId, string newPassword);

    /// <summary>
    /// Invalidates a password reset token for a user.
    /// </summary>
    /// <param name="usersLogin">The user whose reset token should be invalidated.</param>
    /// <returns>A task that returns true if the token is invalidated successfully, otherwise false.</returns>
    Task<bool> InvalidateResetTokenAsync(int userLoginId);

    /// <summary>
    /// Creates a new user login asynchronously.
    /// </summary>
    /// <param name="login">The user login details to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateUserLoginAsync(UsersLogin usersLogin);
}
