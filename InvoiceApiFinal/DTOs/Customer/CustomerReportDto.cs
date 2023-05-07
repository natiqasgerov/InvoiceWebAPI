namespace InvoiceApiFinal.DTOs.Customer
{
    public class CustomerReportDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public ICollection<Models.Invoice>? Invoices { get; set; }
    }
}
