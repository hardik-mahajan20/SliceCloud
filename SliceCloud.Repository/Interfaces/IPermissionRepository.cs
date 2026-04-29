using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IPermissionRepository
{
    /// <summary>
    /// Retrieves all permissions as queryable.
    /// </summary>
    /// <returns>All permissions as queryable.</returns>
    IQueryable<Permission> GetAllPermissionAsQueryable();

    /// <summary>
    /// Retrieves all permissions with modules as queryable.
    /// </summary>
    /// <returns>All permissions with modules as queryable.</returns>
    IQueryable<Permission> GetAllPermissionWithModulesAsQueryable();

    /// <summary>
    /// Retrieves all permissions with roles and modules as queryable.
    /// </summary>
    /// <returns>All permissions with roles and modules as queryable.</returns>
    IQueryable<Permission> GetAllPermissionWithRolesAndModulesAsQueryable();

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
