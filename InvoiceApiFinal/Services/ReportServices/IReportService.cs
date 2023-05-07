using InvoiceApiFinal.DTOs.Customer;
using InvoiceApiFinal.Models;

namespace InvoiceApiFinal.Services.ReportServices
{
    public interface IReportService
    {
        Task<List<CustomerReportDto?>> GetCustomers(int userId);
    }
}
