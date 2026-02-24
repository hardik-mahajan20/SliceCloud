using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Implementations;

public interface ITableService
{
    /// <summary>
    /// Retrieves all tables.
    /// </summary>
    /// <returns>A collection of table view models.</returns>
    Task<List<TableViewModel>> GetAllTablesAsync();

    /// <summary>
    /// Retrieves a paginated list of tables by section ID with an optional search query.
    /// </summary>
    /// <param name="sectionId">The ID of the section to retrieve tables for.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of tables per page.</param>
    /// <param name="searchQuery">An optional search query to filter tables.</param>
    /// <returns>A task that returns a paginated list of table view models.</returns>
    Task<PaginatedList<TableViewModel>> GetPaginatedTablesBySectionIdAsync(int sectionId, int pageNumber, int pageSize, string searchQuery);

    /// <summary>
    /// Retrieves all table IDs for a specific section.
    /// </summary>
    /// <param name="sectionId">The ID of the section to retrieve table IDs for.</param>
    /// <returns>A task that returns a list of table IDs.</returns>
    Task<List<int>> GetAllTableIdsAsync(int sectionId);
}
