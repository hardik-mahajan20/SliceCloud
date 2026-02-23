using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class PermissionService(IPermissionRepository permissionRepository, IRolesRepository rolesRepository) : IPermissionService
{
    IPermissionRepository _permissionRepository = permissionRepository;

    IRolesRepository _rolesRepository = rolesRepository;

    public async Task<RoleAndPermissionsViewModel> GetAllPermissionsAsync(int roleId)
    {
        Role? role = await _rolesRepository.GetRoleByIdAsync(roleId);
        List<Permission>? permissions = await _permissionRepository.GetAllPermissionWithModulesAsQueryable().Where(p => p.RoleId == roleId).ToListAsync();

        RoleAndPermissionsViewModel roleAndPermissionsViewModel = new()
        {
            RoleId = role!.RoleId,
            RoleName = role.RoleName,
            Permissions = permissions.Select(p => new Permissions
            {
                PermissionId = p.PermissionId,
                ModuleId = p.ModuleId,
                ModuleName = p.Module.ModuleName,
                CanView = p.CanView ?? false,
                CanAddEdit = p.CanAddEdit ?? false,
                CanDelete = p.CanDelete ?? false
            }).ToList()
        };
        return roleAndPermissionsViewModel;
    }

    #region RoleHasPermission

    public async Task<bool> RoleHasPermissionAsync(string roleName, string permissionName, int moduleId)
    {
        return await _permissionRepository.GetAllPermissionWithRolesAndModulesAsQueryable()
        .Where(p => p.Role.RoleName == roleName && p.ModuleId == moduleId)
               .AnyAsync(
                   p =>
                       (permissionName == PermissionConstants.CAN_VIEW && p.CanView == true)
                       || (permissionName == PermissionConstants.CAN_ADD_EDIT && p.CanAddEdit == true)
                       || (permissionName == PermissionConstants.CAN_DELETE && p.CanDelete == true)
               );
    }

    #endregion

    #region UpdateAllPermissions

    public async Task<bool> UpdateAllPermissionsAsync(RoleAndPermissionsViewModel roleAndPermissionsViewModel)
    {
        try
        {
            if (roleAndPermissionsViewModel.Permissions == null || roleAndPermissionsViewModel.Permissions.Count == 0)
                return false;

            int roleId = roleAndPermissionsViewModel.RoleId;

            List<int>? permissionIds = roleAndPermissionsViewModel.Permissions
                .Select(p => p.PermissionId)
                .ToList();

            List<Permission>? dbPermissions = await _permissionRepository.GetAllPermissionAsQueryable().Where(p => p.RoleId == roleId && permissionIds.Contains(p.PermissionId)).ToListAsync();

            if (dbPermissions.Count != roleAndPermissionsViewModel.Permissions.Count)
                throw new InvalidOperationException(ErrorConstants.INVALID_PERMISSION_UPDATE_REQUEST);

            foreach (var dbPermission in dbPermissions)
            {
                Permissions? updatedPermission = roleAndPermissionsViewModel.Permissions
                    .First(p => p.PermissionId == dbPermission.PermissionId);

                dbPermission.CanView = updatedPermission.CanView;
                dbPermission.CanAddEdit = updatedPermission.CanAddEdit;
                dbPermission.CanDelete = updatedPermission.CanDelete;
            }

            await _permissionRepository.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    #endregion

}
