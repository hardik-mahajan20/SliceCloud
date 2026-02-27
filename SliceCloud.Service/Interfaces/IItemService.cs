using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IItemService
{
    /// <summary>
    /// Retrieves a paginated list of items by group ID with an optional search query.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve items for.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchQuery">An optional search query to filter items.</param>
    /// <returns>A task that returns a paginated list of item view models.</returns>
    Task<PaginatedList<ItemViewModel>> GetPaginatedItemsByGroupIdAsync(int categoryId, int pageNumber, int pageSize, string searchQuery = "");
}
