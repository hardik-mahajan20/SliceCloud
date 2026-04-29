using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ITableRepository
{
    /// <summary>
    /// Retrieves all tables as queryable.
    /// </summary>
    /// <returns>All tables as queryable.</returns>
    IQueryable<Table> GetAllTablesAsQueryable();

    /// <summary>
    /// Retrieves a table by its ID asynchronously.
    /// </summary>
    /// <param name="tableId">The ID of the table to retrieve.</param>
    /// <returns>A task that returns the table if found in the database, otherwise null.</returns>
    Task<Table?> GetTableByIdAsync(int tableId);

    /// <summary>
    /// Adds a new table asynchronously in the database.
    /// </summary>
    /// <param name="table">The table entity to add.</param>
    /// <returns>A task that returns the ID of the created table.</returns>
    Task<int> AddTableAsync(Table table);

    /// <summary>
    /// Updates an existing table asynchronously in the database.
    /// </summary>
    /// <param name="table">The table to update.</param>
    /// <returns>A task that returns the ID of the updated table.</returns>
    Task<int> UpdateTableAsync(Table table);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
