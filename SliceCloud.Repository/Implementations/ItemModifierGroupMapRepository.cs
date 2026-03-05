using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ItemModifierGroupMapRepository(SliceCloudContext sliceCloudContext) : IItemModifierGroupMapRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region AddItemModifierGroupMap

    public async Task AddItemModifierGroupMapAsync(ItemModifierGroupMap itemModifierGroupMap)
    {
        await _sliceCloudContext.ItemModifierGroupMaps.AddAsync(itemModifierGroupMap);
        await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

    #region GetMappingByItemId

    public async Task<List<ItemModifierGroupMap>> GetMappingByItemIdAsync(int itemId)
    {
        return await _sliceCloudContext.ItemModifierGroupMaps
            .Where(x => x.ItemId == itemId)
            .ToListAsync();
    }

    #endregion
}
