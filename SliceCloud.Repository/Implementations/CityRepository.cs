using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class CityRepository(SliceCloudContext sliceCloudContext) : ICityRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetCitiesByStateId

    public async Task<List<City>> GetCitiesByStateIdAsync(int stateId)
    {
        return await _sliceCloudContext.Cities.Where(c => c.StateId == stateId).ToListAsync();
    }

    #endregion
}
