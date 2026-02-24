using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class TableRepository(SliceCloudContext sliceCloudContext) : ITableRepository
{
    SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllTablesAsQueryable

    public IQueryable<Table> GetAllTablesAsQueryable()
    {
        return _sliceCloudContext.Tables.AsQueryable();
    }

    #endregion

    #region AddTable

    public async Task<bool> AddTableAsync(Table table)
    {
        await _sliceCloudContext.Tables.AddAsync(table);
        return _sliceCloudContext.SaveChanges() > 0;
    }

    #endregion
}
