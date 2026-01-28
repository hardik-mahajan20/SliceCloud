using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class RolesService(IRolesRepository rolesRepository) : IRolesService
{
    private readonly IRolesRepository _rolesRepository = rolesRepository;

    #region GetRoleById

    public async Task<Role?> GetRoleByIdAsync(int roleId)
    {
        return await _rolesRepository.GetRoleByIdAsync(roleId);
    }

    #endregion

    #region GetAllRoles

    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _rolesRepository.GetAllRolesAsync();
    }

    #endregion
}