using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ItemModifierGroupMapService(IItemModifierGroupMapRepository itemModifierGroupMapRepository) : IItemModifierGroupMapService
{
    private readonly IItemModifierGroupMapRepository _itemModifierGroupMapRepository = itemModifierGroupMapRepository;

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
}
