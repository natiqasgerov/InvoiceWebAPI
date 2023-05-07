namespace InvoiceApiFinal.Services.Hash
{
    public class PasswordService
    {
        public static string Hashing(string password)
        {
            string hassed = BCrypt.Net.BCrypt.HashPassword(password);
            return hassed;
        }

        public static bool Checking(string password, string hassedPass)
        {
           return BCrypt.Net.BCrypt.Verify(password, hassedPass);
        }
    }
}
