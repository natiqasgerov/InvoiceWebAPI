namespace InvoiceApiFinal.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string Title { get; set; } = String.Empty;
        public string? Comment { get; set; }
        public string Status { get; set; } = String.Empty;
        public double TotalSum { get; set; } = 0;
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<InvoiceRow> Rows { get; set; } = new List<InvoiceRow>();
    }
}
