using InvoiceApiFinal.Models;

namespace InvoiceApiFinal.DTOs.Invoice
{
    public class InvoiceCreateRequest
    {
        public string Title { get; set; } = String.Empty;
        public string? Comment { get; set; }
    }
}
