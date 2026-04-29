using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IUsersLoginRepository
{
    /// <summary>
    /// Retrieves all usersLogins as queryable.
    /// </summary>
    /// <returns>All usersLogins as queryable.</returns>
    IQueryable<UsersLogin> GetUsersLoginAsQueryable();

    /// <summary>
    /// Retrieves all usersLogins with users as queryable.
    /// </summary>
    /// <returns>All usersLogins with users as queryable.</returns>
    IQueryable<UsersLogin> GetUsersLoginWithUserAsQueryable();

    /// <summary>
    /// Adds a new usersLogin asynchronously in the database.
    /// </summary>
    /// <param name="usersLogin">The usersLogin entity to add.</param>
    /// <returns>A task that returns the ID of the created usersLogin.</returns>
    Task<int> AddUserLoginAsync(UsersLogin usersLogin);

    /// <summary>
    /// Updates an existing usersLogin asynchronously in the database.
    /// </summary>
    /// <param name="usersLogin">The usersLogin to update.</param>
    /// <returns>A task that returns the ID of the updated usersLogin.</returns>
    Task<int> UpdateUsersLoginAsync(UsersLogin usersLogin);
}
