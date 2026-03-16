using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IUsersRepository
{
    /// <summary>
    /// Retrieves all users as queryable.
    /// </summary>
    /// <returns>All users as queryable.</returns>
    IQueryable<User> GetAllUsersAsQueryable();

    /// <summary>
    /// Retrieves a user by its ID asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>A task that returns the user if found in the database, otherwise null.</returns>
    Task<User?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Adds a new user asynchronously in the database.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    /// <returns>A task that returns the ID of the created user.</returns>
    Task<int> AddUserAsync(User user);

    /// <summary>
    /// Updates an existing category asynchronously in the database.
    /// </summary>
    /// <param name="category">The category to update.</param>
    /// <returns>A task that returns the ID of the updated category.</returns>
    Task<int> UpdateUserAsync(User user);
}
