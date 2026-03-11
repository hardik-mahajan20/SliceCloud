using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ModifierService(IModifierGroupModifierMappingsRepository modifierGroupModifierMappingsRepository, IModifierGroupRepository modifierGroupRepository, ICurrentUserService currentUserService, IModifierRepository modifierRepository) : IModifierService
{
    private readonly IModifierGroupModifierMappingsRepository _modifierGroupModifierMappingsRepository = modifierGroupModifierMappingsRepository;

    private readonly IModifierGroupRepository _modifierGroupRepository = modifierGroupRepository;

    private readonly ICurrentUserService _currentUserService = currentUserService;

    private readonly IModifierRepository _modifierRepository = modifierRepository;

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

    #region GetPaginatedModifiersByModifierGroupId

    public async Task<PaginatedList<ModifierViewModel>> GetPaginatedModifiersByModifierGroupId(int modifierGroupId, int pageNumber, int pageSize, string searchQuery = "")
    {
        IQueryable<Modifier>? query = _modifierGroupModifierMappingsRepository.GetAllModifierGroupModifierMappingAsQueryable()
                                                    .Where(mapping => mapping.ModifierGroupId == modifierGroupId && mapping.Modifier.IsDeleted == false)
                                                        .OrderByDescending(mapping => mapping.Modifier.CreatedAt)
                                                            .Select(mgm => mgm.Modifier)
                                                                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            string trimmedSearch = searchQuery.Trim().ToLower();
            query = query.Where(
                mapping =>
                    mapping.ModifierName != null && mapping.ModifierName.ToLower().Contains(trimmedSearch)
            );
        }

        int totalCount = await query.CountAsync();
        List<Modifier>? modifiers = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        List<ModifierViewModel>? modifierViewModels = modifiers.Select(modifier =>
            {

                return new ModifierViewModel
                {
                    ModifierId = modifier.ModifierId,
                    ModifierName = modifier.ModifierName,
                    UnitId = modifier.UnitId,
                    Rate = modifier.Rate,
                    Quantity = modifier.Quantity,
                };
            }).ToList();


        return new PaginatedList<ModifierViewModel>(modifierViewModels, totalCount, pageNumber, pageSize);
    }

    #endregion

    #region AddModifier

    public async Task<int> AddModifierAsync(ModifierSectionViewModel modifierSectionViewModel)
    {
        Modifier menuItem = new()
        {
            ModifierName = modifierSectionViewModel.ModifierName ?? string.Empty,
            UnitId = modifierSectionViewModel.UnitId ?? 0,
            Rate = modifierSectionViewModel.Rate ?? 0,
            Quantity = modifierSectionViewModel.Quantity,
            Description = modifierSectionViewModel.Description,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        int newModifiedId = await _modifierRepository.AddModifierAsync(menuItem);

        if (modifierSectionViewModel.ModifierGroupIds != null && modifierSectionViewModel.ModifierGroupIds.Any())
        {
            List<ModifierGroupModifierMapping>? modifierGroupMappings = modifierSectionViewModel.ModifierGroupIds
                .Select(
                    groupId =>
                        new ModifierGroupModifierMapping
                        {
                            ModifierGroupId = groupId,
                            ModifierId = menuItem.ModifierId
                        }
                )
                .ToList();

            await _modifierGroupModifierMappingsRepository.AddModifierGroupMappingsAsync(modifierGroupMappings);
        }
        return newModifiedId;
    }

    public async Task<ModifierViewModel> GetModifierByIdAsync(int modifierId)
    {
        Modifier? modifier = await _modifierRepository
                                        .GetAllModifiersAsQueryable()
                                            .Include(m => m.ModifierGroupModifierMappings)
                                                .FirstOrDefaultAsync(m => m.ModifierId == modifierId && m.IsDeleted == false);

        if (modifier == null)
        {
            return new ModifierViewModel();
        }

        return new ModifierViewModel
        {
            ModifierId = modifier.ModifierId,
            ModifierName = modifier.ModifierName,
            UnitId = modifier.UnitId,
            Rate = modifier.Rate,
            Quantity = modifier.Quantity,
            Description = modifier.Description,
            ModifierGroupIds = modifier.ModifierGroupModifierMappings.Select(mgm => mgm.ModifierGroupId).ToList()
        };

    }

    #endregion

    #region UpdateModifier

    public async Task<int> UpdateModifierAsync(ModifierSectionViewModel modifierSectionViewModel)
    {
        Modifier modifier = new()
        {
            ModifierId = modifierSectionViewModel.ModifierId ?? 0,
            ModifierName = modifierSectionViewModel.ModifierName ?? string.Empty,
            UnitId = modifierSectionViewModel.UnitId ?? 0,
            Rate = modifierSectionViewModel.Rate ?? 0,
            Quantity = modifierSectionViewModel.Quantity,
            Description = modifierSectionViewModel.Description,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = _currentUserService.UserId
        };

        int modifiedId = await _modifierRepository.UpdateModifierAsync(modifier);

        if (modifierSectionViewModel.ModifierGroupIds != null && modifierSectionViewModel.ModifierGroupIds.Any())
        {
            List<ModifierGroupModifierMapping>? existingMappings = await _modifierGroupModifierMappingsRepository.GetAllModifierGroupModifierMappingAsQueryable().Where(m => m.ModifierId == modifier.ModifierId).ToListAsync();

            List<int>? existingGroupIds = existingMappings.Select(m => m.ModifierGroupId).ToList();

            List<int>? newGroupIds = modifierSectionViewModel.ModifierGroupIds.Except(existingGroupIds).ToList();

            List<ModifierGroupModifierMapping>? mappingsToAdd = newGroupIds
                   .Select(
                       groupId =>
                           new ModifierGroupModifierMapping
                           {
                               ModifierGroupId = groupId,
                               ModifierId = modifier.ModifierId
                           }
                   )
                   .ToList();

            List<ModifierGroupModifierMapping>? mappingsToRemove = existingMappings
                                        .Where(m => !modifierSectionViewModel.ModifierGroupIds.
                                                Contains(m.ModifierGroupId))
                                                .ToList();

            if (mappingsToRemove.Any())
                await _modifierGroupModifierMappingsRepository.RemoveModifierGroupMappingsAsync(mappingsToRemove);


            if (mappingsToAdd.Any())
                await _modifierGroupModifierMappingsRepository.AddModifierGroupMappingsAsync(mappingsToAdd);
        }
        return modifiedId;
    }

    public async Task<bool> DeleteModifierAsync(int modifierId)
    {
        Modifier? modifier = await _modifierRepository
                                        .GetAllModifiersAsQueryable()
                                            .FirstOrDefaultAsync(mod => mod.ModifierId == modifierId);
        if (modifier is null)
        {
            return false;
        }
        modifier.IsDeleted = true;
        modifier.ModifiedAt = DateTime.UtcNow;
        modifier.ModifiedBy = _currentUserService.UserId;

        return await _modifierRepository.UpdateModifierAsync(modifier) > 0;
    }

    #endregion

    #region GetAllModifierIds

    public async Task<List<int>> GetAllModifierIdsAsync(int modifierGroupId)
    {
        return await _modifierGroupModifierMappingsRepository.GetAllModifierGroupModifierMappingAsQueryable()
                                                    .Where(item => item.ModifierGroupId == modifierGroupId)
                                                        .Select(mgm => mgm.Modifier)
                                                        .Where(item => item.IsDeleted == false)
                                                        .Select(modifier => modifier.ModifierId)
                                                        .ToListAsync();
    }

    #endregion

    #region DeleteMultipleMultipleModifier

    public async Task<bool> DeleteMultipleModifierAsync(List<int> modifiersIds)
    {
        List<Modifier>? modifiers = await _modifierRepository.GetAllModifiersAsQueryable().Where(m => modifiersIds.Contains(m.ModifierId)).ToListAsync();

        if (modifiers.Any())
        {
            foreach (Modifier modifier in modifiers)
            {
                modifier.IsDeleted = true;
                modifier.ModifiedAt = DateTime.UtcNow;
                modifier.ModifiedBy = _currentUserService.UserId;
            }
        }
        return await _modifierRepository.SaveChangesAsync() > 0;
    }

    #endregion
}
