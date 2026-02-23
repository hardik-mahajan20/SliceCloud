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


    public async Task<bool> CheckDuplicateSectionNameAsync(string sectionName, int? excludeSectionId = null)
    {
        bool isDuplicated = await _sectionRepository.GetAllSectionsAsQueryable().AnyAsync(
                s =>
                    s.SectionName == sectionName
                    && (excludeSectionId == null || s.SectionId != excludeSectionId)
            );
        return isDuplicated;
    }


    public async Task<bool> AddSectionAsync(SectionViewModel sectionViewModel)
    {
        Section section = new()
        {
            SectionName = sectionViewModel.SectionName,
            SectionOrder = 5,
            Description = sectionViewModel.Description,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        return await _sectionRepository.AddSectionAsync(section);
    }
}
