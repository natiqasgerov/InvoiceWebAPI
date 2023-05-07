using InvoiceApiFinal.Models;

namespace InvoiceApiFinal.DTOs.Invoice
{
    public class InvoiceInfoDto
    {
        public string CustomerName { get; set; } = String.Empty;
        public string CustomerEmail { get; set; } = String.Empty;
        public int InvoiceId { get; set; }
        public string? Comment { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;
        public double TotalSum { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public ICollection<InvoiceRow>? InvoiceRows { get; set; }
    }
}
