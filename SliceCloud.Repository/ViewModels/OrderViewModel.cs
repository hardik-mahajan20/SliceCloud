using SliceCloud.Repository.Enums;

namespace SliceCloud.Repository.ViewModels;

public class OrderViewModel
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? CustomerName { get; set; }

    public string? OrderType { get; set; }

    public OrderStatus Status { get; set; }

    public string? PaymentMode { get; set; }

    public decimal? Rating { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? ItemsCount { get; set; }

    public int? NoOfPersons { get; set; } = 1;
}
