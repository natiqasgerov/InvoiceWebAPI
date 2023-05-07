namespace InvoiceApiFinal.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly HttpContext _httpContext;

        public UserProvider(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext.HttpContext!;
        }

        public UserInCookiee? GetUserInfo()
        {
            if (!_httpContext.User.Claims.Any())
            {
                return null;
                
            }
            var id = int.Parse(_httpContext.User.Claims.First(x => x.Type == "userId").Value);
            var email = _httpContext.User.Claims.First(x => x.Type == "userEmail").Value;
            return new UserInCookiee(id, email);
        }
    }
}
