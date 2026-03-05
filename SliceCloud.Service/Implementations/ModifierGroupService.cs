using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ModifierGroupService(IModifierGroupRepository modifierGroupRepository) : IModifierGroupService
{
    private readonly IModifierGroupRepository _modifierGroupRepository = modifierGroupRepository;

    #region GetAllModifierGroups

    public async Task<List<ModifierGroupViewModel>> GetAllModifierGroupsAsync()
    {
        List<ModifierGroup>? modifierGroups = await _modifierGroupRepository.GetAllModifierGroupsAsQueryable().Where(mg => mg.IsDeleted == false)
                .OrderBy(mg => mg.SortOrder)
                .ToListAsync();

        return modifierGroups.Select(mg => new ModifierGroupViewModel
        {
            ModifierGroupId = mg.ModifierGroupId,
            ModifierGroupName = mg.ModifierGroupName
        }).ToList();
    }

    #endregion

    #region GetModifierGroupsByIds

    public async Task<List<ModifierGroup>> GetModifierGroupsByIdsAsync(List<int> modifierGroupIds)
    {
        return await _modifierGroupRepository.GetAllModifierGroupsAsQueryable()
                                                .Where(g => modifierGroupIds
                                                    .Contains(g.ModifierGroupId))
                                                        .ToListAsync();
    }

    #endregion

    #region GetModifierGroupById

    public async Task<ModifierGroupViewModel> GetModifierGroupByIdAsync(int modifierGroupId)
    {
        ModifierGroup? modifierGroup = await _modifierGroupRepository.GetAllModifierGroupsAsQueryable()
                                                    .Include(mg => mg.ModifierGroupModifierMappings)
                                                        .ThenInclude(mgm => mgm.Modifier)
                                                            .FirstOrDefaultAsync(mg => mg.ModifierGroupId == modifierGroupId);
        if (modifierGroup == null)
        {
            return null!;
        }

        return new ModifierGroupViewModel
        {

            ModifierGroupId = modifierGroup.ModifierGroupId,
            ModifierGroupName = modifierGroup.ModifierGroupName,
            Description = modifierGroup.Description,
            Modifiers = modifierGroup.ModifierGroupModifierMappings
                .Select(mgm => new ModifierViewModel
                {
                    ModifierId = mgm.Modifier.ModifierId,
                    ModifierName = mgm.Modifier.ModifierName,
                    ModifierType = (ModifierType?)mgm.Modifier.ModifierType
                }).ToList()
        };
    }

    #endregion
}
