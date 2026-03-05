using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ModifierGroupRepository(SliceCloudContext sliceCloudContext) : IModifierGroupRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllModifierGroups

    public IQueryable<ModifierGroup> GetAllModifierGroupsAsQueryable()
    {
        return _sliceCloudContext.ModifierGroups.AsQueryable();
    }

    #endregion
}
