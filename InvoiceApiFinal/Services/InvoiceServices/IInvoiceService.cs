using InvoiceApiFinal.DTOs.Invoice;
using InvoiceApiFinal.DTOs.Pagination;
using InvoiceApiFinal.Models;

namespace InvoiceApiFinal.Services.InvoiceServices
{
    public interface IInvoiceService
    {
        Task<Invoice?> CheckInvoice(int customerId, int invoiceId);
        Task<Customer?> CheckCustomer(int userId, int customerId);
        Task<Invoice?> CreateInvoice(Customer customer,InvoiceCreateRequest createRequest);
        Task<Invoice?> UpdateInvoice(Invoice invoice, InvoiceEditRequest editRequest);
        Task<Invoice?> ChangeInvoiceStatus(Invoice invoice, string status);
        Task<Invoice?> DeleteInvoice(Invoice invoice);
        Task<Invoice> ArchiveInvoice(Invoice invoice);
        Task<PaginatedListDto<InvoiceInfoDto>> GetInvoices(int customerId,string? model,string? type,
            string? title,int page,int pageSize);
    }
}
