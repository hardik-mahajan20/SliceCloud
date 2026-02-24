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
}
