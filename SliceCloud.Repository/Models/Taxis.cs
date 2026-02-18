namespace SliceCloud.Repository.Models;

public partial class Taxis
{
    public int TaxId { get; set; }

    public string TaxName { get; set; } = null!;

    public bool? IsEnabled { get; set; }

    public bool? IsInclusive { get; set; }

    public float TaxValue { get; set; }

    public bool? IsDefault { get; set; }

    public string TaxType { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<OrderTaxMapping> OrderTaxMappings { get; set; } = new List<OrderTaxMapping>();
}
