using InvoiceApiFinal.DTOs.User;
using InvoiceApiFinal.Models;

namespace InvoiceApiFinal.Services.UserServices
{
    public interface IUserService
    {
        public Task<User?> GetUser(UserLoginForm loginForm);
        public Task<User?> GetUserByName(string name);
        public Task<User?> GetUserById(int id);
        public Task<bool> CreateUser(User user);
        public Task<User> UpdateUser(UserEditRequest newUser, User lastUser);
        public Task<bool> UpdateOnlyPass(User user, string newPass);
        public Task<string> UpdateToken(User user,string newToken);
        public Task<bool> DeleteUser(User user);
        public Task<User?> CheckUser(UserRegisterForm userRegister);
    }
}

