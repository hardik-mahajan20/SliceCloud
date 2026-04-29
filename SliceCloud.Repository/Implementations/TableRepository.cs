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

    #region GetTableById

    public async Task<Table?> GetTableByIdAsync(int tableId)
    {
        return await _sliceCloudContext.Tables.FirstOrDefaultAsync(table => table.TableId == tableId);
    }

    #endregion

    #region AddTable

    public async Task<int> AddTableAsync(Table table)
    {
        await _sliceCloudContext.Tables.AddAsync(table);
        await _sliceCloudContext.SaveChangesAsync();
        return table.TableId;
    }

    #endregion

    #region UpdateTable

    public async Task<int> UpdateTableAsync(Table table)
    {
        _sliceCloudContext.Tables.Update(table);
        await _sliceCloudContext.SaveChangesAsync();
        return table.TableId;

    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
