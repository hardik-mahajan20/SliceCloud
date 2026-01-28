using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class RolesRepository(SliceCloudContext sliceCloudContext) : IRolesRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllRoles

    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _sliceCloudContext.Roles.ToListAsync();
    }

    #endregion

    #region GetRoleById

    public async Task<Role?> GetRoleByIdAsync(int roleId)
    {
        return await _sliceCloudContext.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
    }

    #endregion
}
