using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class StateRepository(SliceCloudContext sliceCloudContext) : IStateRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetStatesByCountryId

    public async Task<List<Models.State>> GetStatesByCountryIdAsync(int countryId)
    {
        return await _sliceCloudContext.States.Where(s => s.CountryId == countryId).ToListAsync();
    }

    #endregion
}
