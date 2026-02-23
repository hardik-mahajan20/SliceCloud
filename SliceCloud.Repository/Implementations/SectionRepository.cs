using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class SectionRepository(SliceCloudContext sliceCloudContext) : ISectionRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllSectionsAsQueryable

    public IQueryable<Section> GetAllSectionsAsQueryable()
    {
        return _sliceCloudContext.Sections.AsQueryable();
    }

    #endregion

    #region AddSection

    public async Task<bool> AddSectionAsync(Section section)
    {
        await _sliceCloudContext.Sections.AddAsync(section);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion
}
