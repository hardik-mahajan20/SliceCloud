using SliceCloud.Repository.Enums;

namespace SliceCloud.Repository.ViewModels;

public class ModifierItemViewModel
{
    public int OrderedItemId { get; set; }

    public int? ModifierItemId { get; set; }

    public string? ModifierItemName { get; set; }

    public decimal? Price { get; set; }

    public ModifierType? ModifierType { get; set; }
}
