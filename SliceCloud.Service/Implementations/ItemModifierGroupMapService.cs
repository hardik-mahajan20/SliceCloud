using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ItemModifierGroupMapService(IItemModifierGroupMapRepository itemModifierGroupMapRepository, IModifierGroupRepository modifierGroupRepository, IModifierGroupModifierMappingsRepository modifierGroupModifierMappingsRepository, IModifierRepository modifierRepository) : IItemModifierGroupMapService
{
    private readonly IItemModifierGroupMapRepository _itemModifierGroupMapRepository = itemModifierGroupMapRepository;
    private readonly IModifierGroupRepository _modifierGroupRepository = modifierGroupRepository;
    private readonly IModifierRepository _modifierRepository = modifierRepository;
    private readonly IModifierGroupModifierMappingsRepository _modifierGroupModifierMappingsRepository = modifierGroupModifierMappingsRepository;

    #region AddItemModifierGroupMap

    public async Task AddItemModifierGroupMapAsync(ItemModifierGroupMapViewModel itemModifierGroupMapViewModel)
    {
        ItemModifierGroupMap itemModifierGroupMap = new()
        {
            ItemId = itemModifierGroupMapViewModel.ItemId,
            ModifierGroupId = itemModifierGroupMapViewModel.ModifierGroupId,
            MinSelectionRequired = (short?)itemModifierGroupMapViewModel.MinValue,
            MaxSelectionAllowed = (short?)itemModifierGroupMapViewModel.MaxValue
        };

        await _itemModifierGroupMapRepository.AddItemModifierGroupMapAsync(itemModifierGroupMap);
    }

    #endregion

    #region GetMappingByItemId

    public async Task<List<ItemModifierGroupMapViewModel>> GetMappingByItemIdAsync(int itemId)
    {
        List<ItemModifierGroupMap>? mappings = await _itemModifierGroupMapRepository.GetMappingByItemIdAsync(itemId);

        List<int>? groupIds = mappings.Select(x => x.ModifierGroupId).Distinct().ToList();

        List<ModifierGroup>? groups = await _modifierGroupRepository.GetAllModifierGroupsAsQueryable()
           .Where(g => groupIds.Contains(g.ModifierGroupId))
           .ToListAsync();

        List<ModifierGroupModifierMapping>? modifierMappings = await _modifierGroupModifierMappingsRepository.GetAllModifierGroupModifierMappingAsQueryable().Where(x => groupIds.Contains(x.ModifierGroupId))
           .ToListAsync();

        List<int>? modifierIds = modifierMappings.Select(x => x.ModifierId).Distinct().ToList();

        List<Modifier>? modifiers = await _modifierRepository.GetAllModifiersAsQueryable()
                                            .Where(x => modifierIds.Contains(x.ModifierId))
                                            .ToListAsync();

        List<ItemModifierGroupMapViewModel>? result = mappings.Select(map =>
        {
            ModifierGroup? group = groups.FirstOrDefault(g => g.ModifierGroupId == map.ModifierGroupId);

            IEnumerable<int>? groupModifierIds = modifierMappings
                .Where(x => x.ModifierGroupId == map.ModifierGroupId)
                .Select(x => x.ModifierId);

            List<ModifierItemViewModel>? modifierItems = modifiers
                .Where(m => groupModifierIds.Contains(m.ModifierId))
                .Select(m => new ModifierItemViewModel
                {
                    ModifierItemId = m.ModifierId,
                    ModifierItemName = m.ModifierName,
                    Price = m.Rate,
                    ModifierType = (ModifierType?)m.ModifierType
                }).ToList();

            return new ItemModifierGroupMapViewModel
            {
                ItemId = map.ItemId,
                ModifierGroupId = map.ModifierGroupId,
                ModifierGroupName = group?.ModifierGroupName,
                MinValue = map.MinSelectionRequired,
                MaxValue = map.MaxSelectionAllowed,
                ModifierItems = modifierItems
            };
        }).ToList();


        List<ItemModifierGroupMapViewModel>? mappedViewModels = result.Select(m => new ItemModifierGroupMapViewModel
        {
            ItemModifierGroupMapId = m.ItemModifierGroupMapId,
            ItemId = m.ItemId,
            ModifierGroupId = m.ModifierGroupId,
            ModifierGroupName = m.ModifierGroupName,
            MinValue = m.MinValue,
            MaxValue = m.MaxValue,
            ModifierItems = m.ModifierItems,
            ModifierType = m.ModifierType
        }).ToList();

        return mappedViewModels;
    }

    #endregion

    #region DeleteItemModifierGroupMapsByItemId

    public async Task DeleteItemModifierGroupMapsByItemIdAsync(int itemId)
    {
        List<ItemModifierGroupMap>? existingMappings = await _itemModifierGroupMapRepository.GetAllItemModifierGroupMapsAsQueryable()
                                                                                                .Where(m => m.ItemId == itemId)
                                                                                                    .ToListAsync();
        if (existingMappings == null || !existingMappings.Any())
            return;

        _itemModifierGroupMapRepository.RemoveItemModifierGroupMaps(existingMappings);

        await _itemModifierGroupMapRepository.SaveChangesAsync();
    }

    #endregion
}
