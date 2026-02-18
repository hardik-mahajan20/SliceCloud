namespace SliceCloud.Repository.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int OrderId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateTime? InvoiceDate { get; set; }

    public virtual Order Order { get; set; } = null!;
}
