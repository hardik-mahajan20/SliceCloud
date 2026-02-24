using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ITableRepository
{
    /// <summary>
    /// Retrieves all tables as queryable.
    /// </summary>
    /// <returns>A collection of all tables as queryable.</returns>
    IQueryable<Table> GetAllTablesAsQueryable();
}
