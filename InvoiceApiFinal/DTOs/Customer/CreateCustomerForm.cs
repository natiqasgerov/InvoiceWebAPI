namespace InvoiceApiFinal.DTOs.Customer
{
    public class CreateCustomerForm
    {
        public string Name { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;
    }
}
