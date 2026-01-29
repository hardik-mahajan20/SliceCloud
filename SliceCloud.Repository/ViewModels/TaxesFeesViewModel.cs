using System.ComponentModel.DataAnnotations;

namespace SliceCloud.Repository.ViewModels;

public class TaxesFeesViewModel
{
    public int TaxId { get; set; }

    [Required(ErrorMessage = "Tax Name is required.")]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]{2,19}$", ErrorMessage = "Tax Name must start with a letter and be 3-20 characters long, using only letters, numbers, and spaces.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Tax Name must be between 3 and 20 characters.")]
    public string? TaxName { get; set; }

    [Required(ErrorMessage = "Tax type is required.")]
    public string? TaxType { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsDefault { get; set; }

    public bool IsInclusive { get; set; }

    [Required(ErrorMessage = "Tax value is required.")]
    [Range(1, 100, ErrorMessage = "Tax value must be between 0 and 100.")]
    public decimal? TaxValue { get; set; }
}
