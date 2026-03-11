using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class CityRepository(SliceCloudContext sliceCloudContext) : ICityRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllCitiesAsQueryable

    public IQueryable<City> GetAllCitiesAsQueryable()
    {
        return _sliceCloudContext.Cities.AsQueryable();
    }

    #endregion
}
