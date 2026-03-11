using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IUsersLoginRepository
{
    /// <summary>
    /// Retrieves user's login details as queryable.
    /// </summary>
    /// <returns>Login details as queryable if found.</returns>
    IQueryable<UsersLogin> GetUsersLoginAsQueryable();

    /// <summary>
    /// Retrieves a user's login details with user as queryable.
    /// </summary>
    /// <returns>A task that returns the users's login details with user as queryable if found .</returns>
    IQueryable<UsersLogin> GetUsersLoginWithUserAsQueryable();

    /// <summary>
    /// Creates a new user login asynchronously.
    /// </summary>
    /// <param name="usersLogin">The user login details to create.</param>
    /// <returns>A task that returns true if user created.</returns>
    Task<bool> CreateUserLoginAsync(UsersLogin usersLogin);

    /// <summary>
    /// Updates a existing user login asynchronously.
    /// </summary>
    /// <param name="usersLogin">The user login details to update.</param>
    /// <returns>A task that returns true if user updated.</returns>
    Task<bool> UpdateUsersLoginAsync(UsersLogin usersLogin);
}
