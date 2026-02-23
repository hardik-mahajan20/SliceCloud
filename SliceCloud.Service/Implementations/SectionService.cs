using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class SectionService(ISectionRepository sectionRepository, ICurrentUserService currentUserService) : ISectionService
{
    private readonly ISectionRepository _sectionRepository = sectionRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    #region GetAllSections

    public async Task<List<SectionViewModel>> GetAllSections()
    {
        List<Section>? sections = await _sectionRepository.GetAllSectionsAsQueryable().Where(s => !s.IsDeleted ?? false).OrderBy(c => c.SectionOrder).ToListAsync();

        List<SectionViewModel> sectionViewModels = sections
                                                        .Select(s => new SectionViewModel
                                                        {
                                                            SectionId = s.SectionId,
                                                            SectionName = s.SectionName,
                                                            Description = s.Description,
                                                            IsDeleted = s.IsDeleted
                                                        })
                                                        .ToList();

        return sectionViewModels;
    }

    #endregion

    #region GetSectionById

    public async Task<SectionViewModel> GetSectionByIdAsync(int sectionId)
    {
        Section? section = await _sectionRepository.GetSectionByIdAsync(sectionId);

        if (section == null)
        {
            return new SectionViewModel();
        }

        return new SectionViewModel
        {
            SectionId = section.SectionId,
            SectionName = section.SectionName,
            Description = section.Description,
            IsDeleted = section.IsDeleted
        };
    }

    #endregion

    #region CheckDuplicateSectionName

    public async Task<bool> CheckDuplicateSectionNameAsync(string sectionName, int? excludeSectionId = null)
    {
        bool isDuplicated = await _sectionRepository.GetAllSectionsAsQueryable().AnyAsync(
                s =>
                    s.SectionName == sectionName
                    && (excludeSectionId == null || s.SectionId != excludeSectionId)
            );
        return isDuplicated;
    }

    #endregion

    #region AddSection

    public async Task<bool> AddSectionAsync(SectionViewModel sectionViewModel)
    {
        int maxOrder = await _sectionRepository.GetAllSectionsAsQueryable().Where(s => !s.IsDeleted == false).Select(s => (int?)s.SectionOrder).MaxAsync() ?? 0;

        Section section = new()
        {
            SectionName = sectionViewModel.SectionName,
            SectionOrder = maxOrder + 1,
            Description = sectionViewModel.Description,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        return await _sectionRepository.AddSectionAsync(section);
    }

    #endregion

    #region UpdateSection

    public async Task<bool> UpdateSectionAsync(SectionViewModel sectionViewModel)
    {
        Section? section = await _sectionRepository.GetSectionByIdAsync(sectionViewModel.SectionId);
        if (section == null) return false;

        section.SectionName = sectionViewModel.SectionName;
        section.Description = sectionViewModel.Description;
        section.ModifiedAt = DateTime.UtcNow;
        section.ModifiedBy = _currentUserService.UserId;

        return await _sectionRepository.UpdateSectionAsync(section);
    }

    #endregion

    #region DeleteSection

    public async Task<bool> DeleteSectionAsync(int sectionId)
    {
        Section? section = await _sectionRepository.GetSectionByIdAsync(sectionId);
        if (section == null)
        {
            return false;
        }

        section.IsDeleted = true;
        section.ModifiedAt = DateTime.UtcNow;
        section.ModifiedBy = _currentUserService.UserId;

        return await _sectionRepository.UpdateSectionAsync(section);
    }

    #endregion
}
