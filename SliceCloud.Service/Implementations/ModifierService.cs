using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ModifierService(IModifierGroupModifierMappingsRepository modifierGroupModifierMappingsRepository, IModifierGroupRepository modifierGroupRepository) : IModifierService
{
    private readonly IModifierGroupModifierMappingsRepository _modifierGroupModifierMappingsRepository = modifierGroupModifierMappingsRepository;

    private readonly IModifierGroupRepository _modifierGroupRepository = modifierGroupRepository;

    #region GetModifiersByGroupIds

    public async Task<List<Modifier>> GetModifiersByGroupIdsAsync(List<int> modifierGroupIds)
    {
        return await _modifierGroupModifierMappingsRepository.GetAllModifierGroupModifierMappingAsQueryable()
                    .Where(mgm => modifierGroupIds.Contains(mgm.ModifierGroupId))
                        .Select(mgm => mgm.Modifier)
                            .Distinct()
                                .Include(m => m.ModifierGroupModifierMappings)
                                    .ToListAsync();
    }

    #endregion

    #region GetAllModifierGroups

    public async Task<List<ModifierGroup>> GetAllModifierGroupsAsync()
    {
        return await _modifierGroupRepository.GetAllModifierGroupsAsQueryable().ToListAsync();
    }

    #endregion
}
