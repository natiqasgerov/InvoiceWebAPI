namespace InvoiceApiFinal.Services.TokenServices
{
    public class JwtConfig
    {
        public string Secret { get; set; } = String.Empty;
        public string Issuer { get; set; } = String.Empty;
        public string Auidience { get; set; } = String.Empty;
        public int ExpiresInDays { get; set; }
    }
}
