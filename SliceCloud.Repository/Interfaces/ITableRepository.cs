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
}
