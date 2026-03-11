using Microsoft.EntityFrameworkCore;
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
        IQueryable<City>? cities = _cityRepository.GetAllCitiesAsQueryable();

        List<City>? filteredCities = await cities.Where(c => c.StateId == stateId)
                                                    .AsNoTracking()
                                                    .ToListAsync();

        return filteredCities;
    }

    #endregion

}
