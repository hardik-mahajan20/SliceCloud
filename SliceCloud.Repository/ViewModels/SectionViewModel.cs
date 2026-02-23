using System.ComponentModel.DataAnnotations;

namespace SliceCloud.Repository.ViewModels;

public class SectionViewModel
{
    public int SectionId { get; set; }

    [Required(ErrorMessage = "Section name is required.")]
    [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9 ]{0,18}[a-zA-Z0-9]$", ErrorMessage = "Section name must start with a letter and be 2-20 characters long, using only letters, numbers, and spaces.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Section name must be between 2 and 20 characters.")]

    public string SectionName { get; set; } = null!;

    [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public int? WaitingListCount { get; set; }
}
