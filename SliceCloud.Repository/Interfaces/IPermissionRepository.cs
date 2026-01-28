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
}
