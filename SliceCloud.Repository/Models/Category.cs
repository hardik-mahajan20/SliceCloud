namespace SliceCloud.Repository.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public int? SortOrder { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
