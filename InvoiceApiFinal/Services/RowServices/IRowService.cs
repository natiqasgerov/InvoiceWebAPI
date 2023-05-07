using InvoiceApiFinal.DTOs.RowInvoice;
using InvoiceApiFinal.Models;

namespace InvoiceApiFinal.Services.RowServices
{
    public interface IRowService
    {
        Task<Customer?> CheckCustomer(int userId, int customerId);
        Task<Invoice?> CheckInvoice(int customerId, int invoiceId);
        Task<InvoiceRow?> CheckRow(int invoiceId, int rowId);
        Task<InvoiceRow?> AddRow(RowCreateRequest createRequest,Invoice invoice);
        Task<InvoiceRow?> DeleteRow(InvoiceRow invoiceRow,Invoice invoice);
    }
}
