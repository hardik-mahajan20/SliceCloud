using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class CountryRepository(SliceCloudContext sliceCloudContext) : ICountryRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllCountries

    public IQueryable<Country> GetAllCountriesAsQueryable()
    {
        return _sliceCloudContext.Countries.AsQueryable();
    }

    #endregion
}
