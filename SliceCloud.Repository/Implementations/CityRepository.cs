using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class CityRepository(SliceCloudContext sliceCloudContext) : ICityRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllCitiesAsQuearyable

    public IQueryable<City> GetAllCitiesAsQuearyable()
    {
        return _sliceCloudContext.Cities.AsQueryable();
    }

    #endregion
}
