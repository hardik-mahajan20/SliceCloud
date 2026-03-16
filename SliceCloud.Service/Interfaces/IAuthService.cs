using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Authenticate an user by their email and password asynchronously.
    /// </summary>
    /// <param name="userEmail">The email of the user to check.</param>
    /// <param name="userPassword">The password of the user to check.</param>
    /// <returns>A task that returns users login details if the user exists, otherwise returns null.</returns>
    Task<UsersLogin?> AuthenticateUserAsync(string userEmail, string userPassword);

    /// <summary>
    /// Get user login details by their email async asynchronously.
    /// </summary>
    /// <param name="userEmail">The email of the user to check.</param>
    /// <returns>A task that returns users login details if the user exists, otherwise returns null.</returns>
    Task<UsersLogin?> GetUserLoginByEmailAsync(string userEmail);

    /// <summary>
    /// Generates a password reset token for a user.
    /// </summary>
    /// <param name="userEmail">The email of the user to generate the token for.</param>
    /// <returns>A task that returns the generated password reset token.</returns>
    Task<string> GeneratePasswordResetTokenAsync(string userEmail);

    /// <summary>
    /// Validates a password reset token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>A task that returns true if the token is valid, otherwise false.</returns>
    Task<bool> ValidatePasswordResetTokenAsync(string token);

    /// <summary>
    /// Updates a user's password using a password reset token.
    /// </summary>
    /// <param name="token">The password reset token.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <returns>A task that returns true if the password was updated successfully, otherwise false.</returns>
    Task<bool> UpdateUserPasswordAsync(string token, string newPassword);

}
