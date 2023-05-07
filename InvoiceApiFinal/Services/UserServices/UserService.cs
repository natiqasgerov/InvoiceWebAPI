using InvoiceApiFinal.Data;
using InvoiceApiFinal.DTOs.User;
using InvoiceApiFinal.Models;
using InvoiceApiFinal.Services.Hash;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApiFinal.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly InvoiceDbContext _context;

        public UserService(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUser(User user)
        {
            if (user is not null)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<User?> CheckUser(UserRegisterForm userRegister)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Name == userRegister.Name);
        }
        public async Task<User?> GetUser(UserLoginForm form)
        {
            var findUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == form.Name
                && u.Email == form.Email);

            if(findUser is not null && PasswordService.Checking(form.Password, findUser.Password))
                return findUser;
            else 
                return null;
        }
        public async Task<User?> GetUserByName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
        }
        public async Task<User> UpdateUser(UserEditRequest newUser,User lastUser)
        {
            if (!string.IsNullOrWhiteSpace(newUser.Name))
                lastUser.Name = newUser.Name;
            if (!string.IsNullOrWhiteSpace(newUser.Address))
                lastUser.Address = newUser.Address;
            if(!string.IsNullOrWhiteSpace(newUser.Email))
                lastUser.Email = newUser.Email;
            if (!string.IsNullOrWhiteSpace(newUser.PhoneNumber))
                lastUser.PhoneNumber = newUser.PhoneNumber;

            lastUser.UpdatedAt = new DateTimeOffset(DateTime.Now);

            await _context.SaveChangesAsync();
           
            return lastUser;
        }
        public async Task<bool> UpdateOnlyPass(User user,string newPass)
        {
            user.Password = PasswordService.Hashing(newPass);
            user.UpdatedAt = new DateTimeOffset(DateTime.Now);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
            
        }

        public async Task<string> UpdateToken(User user,string newToken)
        {
            user.Token = newToken;
            await _context.SaveChangesAsync();
            return newToken;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
