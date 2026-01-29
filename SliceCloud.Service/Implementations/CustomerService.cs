using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;


    #region GetPaginatedCustomers

    public async Task<PaginatedList<CustomerViewModel>> GetPaginatedCustomersAsync(string search, string status, DateTime? startDate, DateTime? endDate, int page, int pageSize, string sortColumn, string sortDirection)
    {
        PaginatedList<Customer>? customers = await _customerRepository.GetPaginatedCustomersAsync(
            search, status, startDate, endDate, page, pageSize, sortColumn, sortDirection);

        List<CustomerViewModel>? customerViewModel = customers.Select(c => new CustomerViewModel
        {
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
            CreatedDate = c.CreatedAt ?? DateTime.Now,
            PhoneNumber = c.PhoneNo,
            Email = c.Email,
            TotalOrder = c.TotalOrder ?? 0,
        }).ToList();

        return new PaginatedList<CustomerViewModel>(customerViewModel, customers.TotalItems, page, pageSize);
    }

    #endregion

    #region GetCustomerHistory

    public async Task<CustomerHistoryViewModel> GetCustomerHistoryAsync(int customerId)
    {
        Customer? customer = await _customerRepository.GetCustomerWithOrdersAsync(customerId);
        if (customer == null) return new CustomerHistoryViewModel();

        return new CustomerHistoryViewModel
        {
            Name = customer.CustomerName,
            PhoneNumber = customer.PhoneNo,
            MaxOrder = customer.Orders.Any() ? customer.Orders.Max(o => o.TotalAmount) : 0,
            AvgBill = customer.Orders.Any() ? Math.Round(customer.Orders.Average(o => o.TotalAmount), 2) : 0,
            ComingSince = customer.CreatedAt ?? DateTime.Now,
            Visits = customer.Orders.Count,
            Orders = customer.Orders.Select(o => new OrderViewModel
            {
                OrderDate = o.OrderDate ?? DateTime.Now,
                OrderType = o.OrderType ?? "NA",
                PaymentMode = o.PaymentMode ?? "NA",
                ItemsCount = o.OrderedItems.Count,
                TotalAmount = o.TotalAmount
            }).ToList()
        };
    }
    #endregion
}


