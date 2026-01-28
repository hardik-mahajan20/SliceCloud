using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

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
                       (permissionName == "CanView" && p.CanView == true)
                       || (permissionName == "CanAddEdit" && p.CanAddEdit == true)
                       || (permissionName == "CanDelete" && p.CanDelete == true)
               );
    }

    #endregion
}
