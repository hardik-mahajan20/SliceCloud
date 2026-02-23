using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class StateService(IStateRepository stateRepository) : IStateService
{
    private readonly IStateRepository _stateRepository = stateRepository;

    #region GetStatesByCountryId

    public async Task<List<State>> GetStatesByCountryIdAsync(int countryId)
    {
        IQueryable<State>? queary = _stateRepository.GetAllStatesAsQueryable();

        List<State>? states = await queary.Where(s => s.CountryId == countryId).ToListAsync();

        return states;
    }

    #endregion

}
