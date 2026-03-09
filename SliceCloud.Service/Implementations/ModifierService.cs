using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
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
}
