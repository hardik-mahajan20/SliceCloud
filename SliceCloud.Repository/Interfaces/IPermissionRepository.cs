using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IPermissionRepository
{
    /// <summary>
    /// Retrieves all permissions with roles and modules as queryable.
    /// </summary>
    /// <returns>All permissions with roles and modules as queryable.</returns>
    IQueryable<Permission> GetAllPermissionWithRolesAndModulesAsQueryable();

    /// <summary>
    /// Retrieves all permissions with modules as queryable.
    /// </summary>
    /// <returns>All permissions with modules as queryable.</returns>
    IQueryable<Permission> GetAllPermissionWithModulesAsQueryable();

    /// <summary>
    /// Retrieves all permissions as queryable.
    /// </summary>
    /// <returns>All permissions as queryable.</returns>
    IQueryable<Permission> GetAllPermissionAsQueryable();

    /// <summary>
    /// Saves changes of open context asynchronously.
    /// </summary>
    /// <returns>A task that return void as result.</returns>
    Task SaveChangesAsync();
}
