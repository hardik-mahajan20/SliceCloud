using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Repository.Interfaces;

public interface IUsersRepository
{
    /// <summary>
    /// Retrieves all paginated users.
    /// </summary>
    /// <returns>An paginated result of all users.</returns>
    Task<PaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize, string query, string sortOrder, string sortColumn, string search);

    /// <summary>
    /// Checks if an email exists in the database.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="userId">The userId to to neglect.</param>
    /// <returns>A task that returns true if the email exists, otherwise false.</returns>
    Task<bool> IsEmailExistsAsync(string email, int? userId);

    /// <summary>
    /// Checks if a username exists in the database.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <param name="userId">The userId to to neglect.</param>
    /// <returns>A task that returns true if the username exists, otherwise false.</returns>
    Task<bool> IsUsernameExistsAsync(string username, int? userId);

    /// <summary>
    /// Checks if a phone number exists in the database.
    /// </summary>
    /// <param name="phone">The phone number to check.</param>
    /// <param name="userId">The userId to to neglect.</param>
    /// <returns>A task that returns true if the phone number exists, otherwise false.</returns>
    Task<bool> IsPhoneExistsAsync(string phone, int? userId);

    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
    Task<bool> CreateUserAsync(User user);

    /// <summary>
    /// Retrieves a user by their ID asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>A task that returns the user if found, otherwise null.</returns>
    Task<User?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Updates a user's information asynchronously.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateUserAsync(User user);

    /// <summary>
    /// Deletes a user by their ID asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
    Task<bool> DeleteExistingUserAsync(int userId);
}
