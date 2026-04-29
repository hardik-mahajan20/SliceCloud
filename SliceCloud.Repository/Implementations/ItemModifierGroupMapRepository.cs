using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ItemModifierGroupMapRepository(SliceCloudContext sliceCloudContext) : IItemModifierGroupMapRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllItemModifierGroupMapsAsQueryable

    public IQueryable<ItemModifierGroupMap> GetAllItemModifierGroupMapsAsQueryable()
    {
        return _sliceCloudContext.ItemModifierGroupMaps.AsQueryable();
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

    #region AddItemModifierGroupMap

    public async Task<int> AddItemModifierGroupMapAsync(ItemModifierGroupMap itemModifierGroupMap)
    {
        await _sliceCloudContext.ItemModifierGroupMaps.AddAsync(itemModifierGroupMap);
        await _sliceCloudContext.SaveChangesAsync();
        return itemModifierGroupMap.ItemModifierGroupId;
    }

    #endregion

    #region RemoveItemModifierGroupMaps

    public void RemoveItemModifierGroupMaps(IEnumerable<ItemModifierGroupMap> itemModifierGroupMaps)
    {
        _sliceCloudContext.ItemModifierGroupMaps.RemoveRange(itemModifierGroupMaps);
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
