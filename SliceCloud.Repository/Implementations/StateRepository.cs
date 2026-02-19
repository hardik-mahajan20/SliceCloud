using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class StateRepository(SliceCloudContext sliceCloudContext) : IStateRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllStatesAsQueryable

    public IQueryable<State> GetAllStatesAsQueryable()
    {
        return _sliceCloudContext.States.AsQueryable();
    }

    #endregion
}
