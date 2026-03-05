using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ModifierRepository(SliceCloudContext sliceCloudContext) : IModifierRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetModifierGroupModifierMapping

    public IQueryable<ModifierGroupModifierMapping> GetModifierGroupModifierMappingAsQueryable()
    {
        return _sliceCloudContext.ModifierGroupModifierMappings.AsQueryable();
    }

    #endregion
}
