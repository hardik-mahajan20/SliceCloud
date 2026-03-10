using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ModifierGroupModifierMappingsRepository(SliceCloudContext sliceCloudContext) : IModifierGroupModifierMappingsRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllModifierGroupModifierMappingAsQueryable

    public IQueryable<ModifierGroupModifierMapping> GetAllModifierGroupModifierMappingAsQueryable()
    {
        return _sliceCloudContext.ModifierGroupModifierMappings.AsQueryable();
    }

    #endregion

    #region AddModifierGroupMappings

    public async Task AddModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings)
    {
        await _sliceCloudContext.ModifierGroupModifierMappings.AddRangeAsync(modifierGroupModifierMappings);
        await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

    #region RemoveModifierGroupMappings

    public async Task RemoveModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings)
    {
        _sliceCloudContext.ModifierGroupModifierMappings.RemoveRange(modifierGroupModifierMappings);
        await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion
}
