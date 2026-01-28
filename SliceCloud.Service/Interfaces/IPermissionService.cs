using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IPermissionService
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
    /// Retrieves all permissions for a specific role ID asynchronously.
    /// </summary>
    /// <param name="roleId">The ID of the role to retrieve permissions for.</param>
    /// <returns>A task that returns the role and its permissions.</returns>
    Task<RoleAndPermissionsViewModel> GetAllPermissionsAsync(int roleId);


    /// <summary>
    /// Updates all permissions for a specific role asynchronously.
    /// </summary>
    /// <param name="roleNPermission">The role and permission data to update.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateAllPermissionsAsync(RoleAndPermissionsViewModel roleAndPermissionsViewModel);
}
