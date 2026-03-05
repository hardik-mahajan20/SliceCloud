using SliceCloud.Repository.Enums;

namespace SliceCloud.Repository.ViewModels;

public class ItemModifierGroupMapViewModel
{
    public int ItemModifierGroupMapId { get; set; }

    public int ItemId { get; set; }

    public int ModifierGroupId { get; set; }

    public string? ModifierGroupName { get; set; }

    public int? MinValue { get; set; }

    public int? MaxValue { get; set; }

    public List<ModifierItemViewModel> ModifierItems { get; set; } = new List<ModifierItemViewModel>();

    public ModifierType? ModifierType { get; set; }
}
