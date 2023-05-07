namespace InvoiceApiFinal.Models
{
    public class InvoiceRow
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; } = String.Empty;
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public double Sum { get; set; }

    }
}
