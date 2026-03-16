using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class PermissionRepository(SliceCloudContext sliceCloudContext) : IPermissionRepository
{
    SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllPermissionAsQueryable

    public IQueryable<Permission> GetAllPermissionAsQueryable()
    {
        return _sliceCloudContext.Permissions.AsQueryable();
    }

    #endregion

    #region GetAllPermissionWithModulesAsQueryable

    public IQueryable<Permission> GetAllPermissionWithModulesAsQueryable()
    {
        return _sliceCloudContext.Permissions.Include(p => p.Module).AsQueryable();
    }

    #endregion

    #region GetAllPermissionWithRolesAndModulesAsQueryable

    public IQueryable<Permission> GetAllPermissionWithRolesAndModulesAsQueryable()
    {
        return _sliceCloudContext.Permissions.Include(p => p.Role).Include(p => p.Module).AsQueryable();
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
