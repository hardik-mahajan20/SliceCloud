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

    #region 

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

        int newModifieId = await _modifierRepository.AddModifierAsync(menuItem);

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
        return newModifieId;
    }

    #endregion

}
