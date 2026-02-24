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

    /// <summary>
    /// Checks if a table name exists in a specific section, optionally excluding a specific table by ID.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <param name="sectionId">The ID of the section to check in.</param>
    /// <param name="excludeTableId">The ID of the table to exclude from the check (optional).</param>
    /// <returns>True if the table name exists, otherwise false.</returns>
    Task<bool> IsDuplicateTableNameAsync(string tableName, int sectionId, int? excludeTableId = null);

    /// <summary>
    /// Adds a new table asynchronously.
    /// </summary>
    /// <param name="tableViewModel">The view model containing table details.</param>
    /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
    Task<bool> AddTableAsync(TableViewModel tableViewModel);

    /// <summary>
    /// Retrieves a table by its ID asynchronously.
    /// </summary>
    /// <param name="tableId">The ID of the table to retrieve.</param>
    /// <returns>The table if found, otherwise null.</returns>
    Task<Repository.Models.Table?> GetTableByIdAsync(int tableId);

    /// <summary>
    /// Updates an existing table asynchronously.
    /// </summary>
    /// <param name="tableViewModel">The tableViewModel to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> UpdateTableAsync(TableViewModel tableViewModel);
}
