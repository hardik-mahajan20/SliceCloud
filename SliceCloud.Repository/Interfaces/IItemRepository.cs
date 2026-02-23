using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IItemRepository
{
    /// <summary>
    /// Retrieves all items as queryable.
    /// </summary>
    /// <returns>All items as queryable.</returns>
    IQueryable<Item> GetAllItemsAsQueryable();

}
