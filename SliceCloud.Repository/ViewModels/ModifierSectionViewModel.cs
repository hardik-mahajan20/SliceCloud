using System.ComponentModel.DataAnnotations;

namespace SliceCloud.Repository.ViewModels;

public class ModifierSectionViewModel
{
    public string? ModifierName { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be a positive number.")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    public int? UnitId { get; set; }

    [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
    //  [RegularExpression(@"^[A-Z][A-Za-z0-9\s,.]*$", ErrorMessage = "Description must start with an uppercase letter and can contain letters, numbers, spaces, commas, and periods.")]
    public string? Description { get; set; }

    public IEnumerable<ModifierGroupViewModel?>? ModifierGroups { get; set; }

    public IEnumerable<UnitViewModel?>? Units { get; set; }

    [Required(ErrorMessage = "Please select at least one modifier group.")]
    [MinLength(1, ErrorMessage = "Please select at least one modifier group.")]
    public List<int>? ModifierGroupIds { get; set; }
}
