namespace SliceCloud.Repository.Models;

public partial class ModifierGroup
{
    public int ModifierGroupId { get; set; }

    public int? SortOrder { get; set; }

    public string ModifierGroupName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<ItemModifierGroupMap> ItemModifierGroupMaps { get; set; } = new List<ItemModifierGroupMap>();

    public virtual ICollection<ModifierGroupModifierMapping> ModifierGroupModifierMappings { get; set; } = new List<ModifierGroupModifierMapping>();
}
