using Microsoft.EntityFrameworkCore;
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

    #region GetTableById

    public async Task<Table?> GetTableByIdAsync(int tableId)
    {
        return await _sliceCloudContext.Tables.FirstOrDefaultAsync(table => table.TableId == tableId);
    }

    #endregion

    #region UpdateTable

    public async Task<bool> UpdateTableAsync(Table table)
    {
        _sliceCloudContext.Tables.Update(table);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion
}
