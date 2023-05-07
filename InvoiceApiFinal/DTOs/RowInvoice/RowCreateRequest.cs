namespace InvoiceApiFinal.DTOs.RowInvoice
{
    public class RowCreateRequest
    {
        public string Description { get; set; } = String.Empty;
        public double Quantity { get; set; }
        public double Amount { get; set; }
    }
}
