using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IPermissionRepository
{
    /// <summary>
    /// Checks if a role has a specific permission for a given module.
    /// </summary>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="permissionName">The name of the permission.</param>
    /// <param name="moduleId">The ID of the module.</param>
    /// <returns>A task that returns true if the role has the permission, otherwise false.</returns>
    Task<bool> RoleHasPermissionAsync(string roleName, string permissionName, int moduleId);

    /// <summary>
    /// Retrieves all permissions associated with a specific role ID asynchronously.
    /// </summary>
    /// <param name="roleId">The ID of the role to retrieve permissions for.</param>
    /// <returns>A task that returns a list of permissions.</returns>
    Task<List<Permission>> GetAllPermissionsByRoleIdAsync(int roleId);

    /// <summary>
    /// Retrieves all permissions associated with a specific role ID and permissionIds asynchronously.
    /// </summary>
    /// <param name="roleId">The ID of the role to retrieve permissions for.</param>
    /// <param name="permissionIds">The permissionIds of the role to retrieve permissions for.</param>
    /// <returns>A task that returns a list of permissions.</returns>
    Task<List<Permission>> GetPermissionsByRoleAsync(int roleId, List<int> permissionIds);

    /// <summary>
    /// Saves changes of open context asynchronously.
    /// </summary>
    /// <returns>A task that return void as result.</returns>
    Task SaveChangesAsync();
}
