using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ItemRepository(SliceCloudContext sliceCloudContext) : IItemRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllItemsAsQueryable

    public IQueryable<Item> GetAllItemsAsQueryable()
    {
        return _sliceCloudContext.Items.AsQueryable();
    }

    #endregion

    #region GetItemById

    public async Task<Item?> GetItemByIdAsync(int itemId)
    {
        return await _sliceCloudContext.Items.FirstOrDefaultAsync(m => m.ItemId == itemId && m.IsDeleted == false);
    }

    #endregion

    #region AddMenuItem

    public async Task<int> AddMenuItemAsync(Item item)
    {
        await _sliceCloudContext.Items.AddAsync(item);
        await _sliceCloudContext.SaveChangesAsync();
        return item.ItemId;
    }

    #endregion

    #region UpdateMenuItem

    public async Task<int> UpdateMenuItemAsync(Item item)
    {
        _sliceCloudContext.Items.Update(item);
        await _sliceCloudContext.SaveChangesAsync();
        return item.ItemId;
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
