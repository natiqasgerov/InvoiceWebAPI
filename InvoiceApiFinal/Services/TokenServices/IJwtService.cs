namespace InvoiceApiFinal.Services.TokenServices
{
    public interface IJwtService
    {
        string GenerateSecurityToken(int id,string email);
    }
}
