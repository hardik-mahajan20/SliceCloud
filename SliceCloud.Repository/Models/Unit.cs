namespace SliceCloud.Repository.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public string UnitName { get; set; } = null!;

    public string? ShortName { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
