using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Interfaces;

public interface IRolesService
{
    /// <summary>
    /// Retrieves a role by its ID asynchronously.
    /// </summary>
    /// <param name="roleId">The ID of the role to retrieve.</param>
    /// <returns>A task that returns the role if found, otherwise null.</returns>
    Task<Role?> GetRoleByIdAsync(int roleId);


    /// <summary>
    /// Retrieves all the roles asynchronously.
    /// </summary>
    /// <returns>A task that returns a list of roles.</returns>
    Task<List<Role>> GetAllRolesAsync();
}
