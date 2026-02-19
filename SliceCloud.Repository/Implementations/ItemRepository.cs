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

}
