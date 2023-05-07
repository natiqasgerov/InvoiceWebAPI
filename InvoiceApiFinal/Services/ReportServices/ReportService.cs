using InvoiceApiFinal.Data;
using InvoiceApiFinal.DTOs.Customer;
using InvoiceApiFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApiFinal.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly InvoiceDbContext _context;
        public ReportService(InvoiceDbContext context)
        {
            _context = context;
        }
   
        public async Task<List<CustomerReportDto?>> GetCustomers(int userId)
        {
            var customerDtoList = new List<CustomerReportDto>();

            var customer = await _context.Customers.Include(x => x.Invoices)
                .Where(x => x.UserId == userId && x.IsDeleted == false).ToListAsync();

            foreach (var item in customer)
            {
                customerDtoList.Add(new CustomerReportDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    CreatedAt = item.CreatedAt,
                    Invoices = item.Invoices,
                });
            }

            foreach (var item in customerDtoList)
            {
                item.Invoices = _context.Invoices.Include(x => x.Rows)
                    .Where(x => x.CustomerId == item.Id && x.IsDeleted == false).ToList();
            }

            if (customerDtoList.Count == 0)
                return null;

            return customerDtoList;
        }
    }
}
