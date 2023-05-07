namespace InvoiceApiFinal.Providers
{
    public class UserInCookiee
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public UserInCookiee(int id, string email)
        {
            Id = id;
            Email = email;
        }
    }
}
