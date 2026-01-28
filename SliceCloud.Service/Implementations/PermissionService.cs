using SliceCloud.Repository.Interfaces;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class PermissionService(IPermissionRepository permissionRepository) : IPermissionService
{
    IPermissionRepository _permissionRepository = permissionRepository;

    #region RoleHasPermission

    public async Task<bool> RoleHasPermissionAsync(string roleName, string permissionName, int moduleId)
    {
        return await _permissionRepository.RoleHasPermissionAsync(roleName, permissionName, moduleId);
    }

    #endregion
}
