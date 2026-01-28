using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CountryService(ICountryRepository countryRepository) : ICountryService
{
    private readonly ICountryRepository _countryRepository = countryRepository;

    #region GetAllCountries

    public async Task<List<Country>> GetAllCountriesAsync()
    {
        return await _countryRepository.GetAllCountriesAsync();
    }

    #endregion
}
