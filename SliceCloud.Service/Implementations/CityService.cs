using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CityService(ICityRepository cityRepository) : ICityService
{
    private readonly ICityRepository _cityRepository = cityRepository;

    #region GetCitiesByStateId

    public async Task<List<City>> GetCitiesByStateIdAsync(int stateId)
    {
        return await _cityRepository.GetCitiesByStateIdAsync(stateId);
    }

    #endregion

}
