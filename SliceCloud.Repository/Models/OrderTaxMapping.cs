namespace SliceCloud.Repository.Models;

public partial class OrderTaxMapping
{
    public int OrderTaxId { get; set; }

    public int OrderId { get; set; }

    public int TaxId { get; set; }

    public float? TaxValue { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Taxis Tax { get; set; } = null!;
}
