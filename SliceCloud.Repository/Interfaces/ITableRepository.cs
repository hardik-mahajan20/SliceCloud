using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ITableRepository
{
    /// <summary>
    /// Retrieves all tables as queryable.
    /// </summary>
    /// <returns>A collection of all tables as queryable.</returns>
    IQueryable<Table> GetAllTablesAsQueryable();

    /// <summary>
    /// Adds a new table asynchronously.
    /// </summary>
    /// <param name="table">The table to add.</param>
    /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
    Task<bool> AddTableAsync(Table table);

    /// <summary>
    /// Retrieves a table by its ID asynchronously.
    /// </summary>
    /// <param name="tableId">The ID of the table to retrieve.</param>
    /// <returns>The table if found, otherwise null.</returns>
    Task<Table?> GetTableByIdAsync(int tableId);

    /// <summary>
    /// Updates an existing table asynchronously.
    /// </summary>
    /// <param name="table">The table to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> UpdateTableAsync(Table table);
}
