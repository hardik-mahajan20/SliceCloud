using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class CountryRepository(SliceCloudContext sliceCloudContext) : ICountryRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllCountries

    public async Task<List<Country>> GetAllCountriesAsync()
    {
        return await _sliceCloudContext.Countries.ToListAsync();
    }

    #endregion
}
