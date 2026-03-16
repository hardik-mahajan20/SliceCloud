using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class UnitRepository(SliceCloudContext sliceCloudContext) : IUnitRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllUnits

    public IQueryable<Unit> GetAllUnitsAsQueryable()
    {
        return _sliceCloudContext.Units.AsQueryable();
    }

    #endregion

}
