using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IRolesRepository
{
    /// <summary>
    /// Retrieves all roles as queryable.
    /// </summary>
    /// <returns>All roles as queryable.</returns>
    IQueryable<Role> GetAllRolesAsQueryable();

    /// <summary>
    /// Retrieves a role by its ID asynchronously.
    /// </summary>
    /// <param name="roleId">The ID of the role to retrieve.</param>
    /// <returns>A task that returns the role if found in the database, otherwise null.</returns>
    Task<Role?> GetRoleByIdAsync(int roleId);
}
