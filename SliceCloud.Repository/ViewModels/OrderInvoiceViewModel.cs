namespace SliceCloud.Repository.ViewModels;

public class OrderInvoiceViewModel
{
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerEmail { get; set; }

        public int NoOfPersons { get; set; }

        public decimal TableCapacity { get; set; }

        public string? InvoiceNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? PaidOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? OrderDuration { get; set; }

        public string? OrderStatus { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();

        public decimal SubTotal { get; set; }

        public decimal CGST { get; set; }

        public decimal SGST { get; set; }

        public decimal GST { get; set; }

        public decimal Other { get; set; }

        public decimal TotalAmountDue { get; set; }

        public string? PaymentMethod { get; set; }

        public List<TaxBreakdownViewModel> TaxBreakdown { get; set; } = new();

        public List<string> Tables { get; set; } = new List<string>();

        public List<string> Sections { get; set; } = new List<string>();
}
