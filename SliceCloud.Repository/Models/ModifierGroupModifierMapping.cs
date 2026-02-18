namespace SliceCloud.Repository.Models;

public partial class ModifierGroupModifierMapping
{
    public int ModifierGroupModifierMappingId { get; set; }

    public int ModifierGroupId { get; set; }

    public int ModifierId { get; set; }

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual ModifierGroup ModifierGroup { get; set; } = null!;
}
