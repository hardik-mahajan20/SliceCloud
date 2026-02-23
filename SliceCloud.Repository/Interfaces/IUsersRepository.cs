using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IUsersRepository
{
    /// <summary>
    /// Retrieves all users as queryable.
    /// </summary>
    /// <returns>All users as queryable.</returns>
    IQueryable<User> GetAllUsersAsQuearyable();

    /// <summary>
    /// Retrieves a user by their ID asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>A task that returns the user if found, otherwise null.</returns>
    Task<User?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that returns true if the creation was successful, otherwise false.</returns>
    Task<bool> CreateUserAsync(User user);

    /// <summary>
    /// Updates a user's information asynchronously.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateUserAsync(User user);
}
