using System.ComponentModel.DataAnnotations;

namespace SliceCloud.Repository.ViewModels;

public class ModifierGroupViewModel
{
    public int ModifierGroupId { get; set; }

    [Required(ErrorMessage = "Modifier Group Name is required.")]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]{2,19}$", ErrorMessage = "Modifier Group Name must start with a letter and be 3-20 characters long, using only letters, numbers, and spaces.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Modifier Group Name must be between 3 and 20 characters.")]
    public string? ModifierGroupName { get; set; } = null!;

    [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
    //[RegularExpression(@"^[A-Z][A-Za-z0-9\s,.]*$", ErrorMessage = "Description must start with an uppercase letter and can contain letters, numbers, spaces, commas, and periods.")]
    public string? Description { get; set; }

    public List<int>? ModifierIds { get; set; }

    public List<ModifierViewModel>? Modifiers { get; set; }

    public List<ModifierItemViewModel> ModifierItems { get; set; } = new List<ModifierItemViewModel>();
}
