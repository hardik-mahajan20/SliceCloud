using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.Constants;

namespace SliceCloud.Repository.Implementations;

public class PermissionRepository(SliceCloudContext sliceCloudContext) : IPermissionRepository
{
    SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region RoleHasPermission

    public async Task<bool> RoleHasPermissionAsync(string roleName, string permissionName, int moduleId)
    {
        return await _sliceCloudContext.Permissions
               .Include(p => p.Role)
               .Include(p => p.Module)
               .Where(p => p.Role.RoleName == roleName && p.ModuleId == moduleId)
               .AnyAsync(
                   p =>
                       (permissionName == PermissionConstants.CAN_VIEW && p.CanView == true)
                       || (permissionName == PermissionConstants.CAN_ADD_EDIT && p.CanAddEdit == true)
                       || (permissionName == PermissionConstants.CAN_DELETE && p.CanDelete == true)
               );
    }

    #endregion

    #region GetAllPermissionsById

    public async Task<List<Permission>> GetAllPermissionsByRoleIdAsync(int roleId)
    {
        return await _sliceCloudContext.Permissions
                .Include(p => p.Module)
                .Where(p => p.RoleId == roleId)
                .ToListAsync();
    }

    #endregion

    #region GetPermissionsByRole

    public async Task<List<Permission>> GetPermissionsByRoleAsync(int roleId, List<int> permissionIds)
    {
        return await _sliceCloudContext.Permissions
            .Where(p => p.RoleId == roleId && permissionIds.Contains(p.PermissionId))
            .ToListAsync();
    }

    #endregion

    #region SaveChanges

    public async Task SaveChangesAsync()
    {
        await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
