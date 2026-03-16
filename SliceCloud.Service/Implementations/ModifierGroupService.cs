using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ModifierGroupService(IModifierGroupRepository modifierGroupRepository, ICurrentUserService currentUserService) : IModifierGroupService
{
    private readonly IModifierGroupRepository _modifierGroupRepository = modifierGroupRepository;

    private readonly ICurrentUserService _currentUserService = currentUserService;

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

    public async Task UpdateModifierGroupOrderAsync(List<int> orderedModifierGroupIds)
    {
        List<ModifierGroup>? modifierGroups = await _modifierGroupRepository.GetAllModifierGroupsAsQueryable()
                                .Where(s => orderedModifierGroupIds.Contains(s.ModifierGroupId) && !(s.IsDeleted ?? false))
                                    .ToListAsync();

        Dictionary<int, ModifierGroup>? modifierGroupDictionary = modifierGroups.ToDictionary(s => s.ModifierGroupId);

        for (int i = 0; i < orderedModifierGroupIds.Count; i++)
        {
            if (modifierGroupDictionary.TryGetValue(orderedModifierGroupIds[i], out var section))
            {
                section.SortOrder = i + 1;
            }
        }

        await _modifierGroupRepository.SaveChangesAsync();
    }

    public async Task<int> AddModifierGroupAsync(ModifierGroupViewModel modifierGroupViewModel)
    {
        bool isModifierGroupNameExists = await _modifierGroupRepository.GetAllModifierGroupsAsQueryable().AsNoTracking()
                        .AnyAsync(c => c.ModifierGroupName == modifierGroupViewModel.ModifierGroupName && (c.IsDeleted == false));

        if (isModifierGroupNameExists)
        {
            throw new InvalidOperationException("A modifier group with the same name already exists.");
        }

        int maxOrder = await _modifierGroupRepository.GetAllModifierGroupsAsQueryable().Where(s => s.IsDeleted == false).Select(s => s.SortOrder).MaxAsync() ?? 0;

        ModifierGroup modifierGroup = new()
        {
            ModifierGroupName = modifierGroupViewModel.ModifierGroupName ?? string.Empty,
            Description = modifierGroupViewModel.Description,
            IsDeleted = false,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow,
            SortOrder = maxOrder + 1
        };

        return await _modifierGroupRepository.AddModifierGroupAsync(modifierGroup);
    }

    public async Task<bool> UpdateModifierGroupAsync(ModifierGroupViewModel modifierGroupViewModel)
    {
        ModifierGroup? modifierGroup = await _modifierGroupRepository.GetModifierGroupByIdAsync(modifierGroupViewModel.ModifierGroupId);

        if (modifierGroup == null)
        {
            throw new KeyNotFoundException("Modifier Group not found.");
        }

        bool isModifierGroupNameExists = await _modifierGroupRepository.GetAllModifierGroupsAsQueryable().AsNoTracking()
                      .AnyAsync(c => c.ModifierGroupName == modifierGroupViewModel.ModifierGroupName && (c.IsDeleted == false) && c.ModifierGroupId != modifierGroupViewModel.ModifierGroupId);

        if (isModifierGroupNameExists)
        {
            throw new InvalidOperationException("A Modifier group with the same name already exists.");
        }

        modifierGroup.ModifierGroupName = modifierGroupViewModel.ModifierGroupName ?? string.Empty;
        modifierGroup.Description = modifierGroupViewModel.Description;
        modifierGroup.ModifiedBy = _currentUserService.UserId;
        modifierGroup.ModifiedAt = DateTime.UtcNow;

        return await _modifierGroupRepository.UpdateModifierGroupAsync(modifierGroup) > 0;
    }

    public async Task<bool> DeleteModifierGroupAsync(int modifierGroupId)
    {

        ModifierGroup? modifierGroup = await _modifierGroupRepository.GetModifierGroupByIdAsync(modifierGroupId);

        if (modifierGroup == null)
        {
            return false;
        }

        modifierGroup.IsDeleted = true;
        modifierGroup.ModifiedAt = DateTime.UtcNow;
        modifierGroup.ModifiedBy = _currentUserService.UserId;

        return await _modifierGroupRepository.UpdateModifierGroupAsync(modifierGroup) > 0;
    }
}
