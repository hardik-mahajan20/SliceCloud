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

    public async Task<int> AddModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings)
    {
        await _sliceCloudContext.ModifierGroupModifierMappings.AddRangeAsync(modifierGroupModifierMappings);
        await _sliceCloudContext.SaveChangesAsync();
        return modifierGroupModifierMappings.FirstOrDefault()?.ModifierGroupModifierMappingId ?? 0;
    }

    #endregion

    #region RemoveModifierGroupMappings

    public async Task<bool> RemoveModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings)
    {
        _sliceCloudContext.ModifierGroupModifierMappings.RemoveRange(modifierGroupModifierMappings);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

}
