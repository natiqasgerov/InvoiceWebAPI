using InvoiceApiFinal.Data;
using InvoiceApiFinal.DTOs.Customer;
using InvoiceApiFinal.DTOs.Pagination;
using InvoiceApiFinal.Models;
using InvoiceApiFinal.Services.Hash;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InvoiceApiFinal.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly InvoiceDbContext _context;

        public CustomerService(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> ArchiveCustomer(int userId, int customerId)
        {
            var user = await _context.Users.Include(u => u.Customers).FirstOrDefaultAsync(u => u.Id == userId);

            var findCustomer = user?.Customers.FirstOrDefault(c => c.Id == customerId && c.IsDeleted == false);

            if (findCustomer is null)
                return null;

            findCustomer.IsDeleted = true;
            findCustomer.DeletedAt = new DateTimeOffset(DateTime.Now);

            await _context.SaveChangesAsync();

            return findCustomer;

        }

        public async Task<CustomerDto> CreateCustomer(int userId, CreateCustomerForm form)
        {
            var customer = new Customer()
            {
                UserId = userId,
                Name = form.Name,
                Address = form.Address,
                Email = form.Email,
                Password = PasswordService.Hashing(form.Password),
                PhoneNumber = form.PhoneNumber,
                CreatedAt = new DateTimeOffset(DateTime.Now),
                UpdatedAt = new DateTimeOffset(DateTime.Now)
            };

            var added = _context.Customers.Add(customer).Entity;
            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                Id = added.Id,
                Name = added.Name,
                Address = added.Address,
                Email = added.Email,
                PhoneNumber = added.PhoneNumber,
                CreatedAt = added.CreatedAt,
                UpdatedAt = added.UpdatedAt
            };
        }

        public async Task<Customer?> DeleteCustomerById(int userId, int customerId)
        {
            var user = await _context.Users.Include(u => u.Customers).FirstOrDefaultAsync(u => u.Id == userId);

            var findCustomer = user?.Customers.FirstOrDefault(c => c.Id == customerId && c.IsDeleted == false);

            if (findCustomer is null)
                return null;

            findCustomer = _context.Customers.Remove(findCustomer).Entity;
            await _context.SaveChangesAsync();

            return findCustomer;
        }

        public async Task<Customer?> GetCustomerById(int userId,int customerId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId && c.UserId == userId && c.IsDeleted == false);
        }


        public async Task<CustomerDto?> UpdateCustomer(int userId, int customerId, CustomerEditRequest editRequest)
        {
            var lastCustomer = await GetCustomerById(userId, customerId);
            if(lastCustomer is null)
                return null;

            if (!string.IsNullOrWhiteSpace(editRequest.Name))
            {
                lastCustomer.Name = editRequest.Name;
            }
            if (!string.IsNullOrWhiteSpace(editRequest.Address))
            {
                lastCustomer.Address = editRequest.Address;
            }
            if(!string.IsNullOrWhiteSpace(editRequest.Email))
            {
                lastCustomer.Email = editRequest.Email;
            }
            if (!string.IsNullOrWhiteSpace(editRequest.PhoneNumber))
            {
                lastCustomer.PhoneNumber = editRequest.PhoneNumber;
            }
          
            lastCustomer.UpdatedAt = new DateTimeOffset(DateTime.Now);

            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                Id = lastCustomer.Id,
                Name = lastCustomer.Name,
                Address = lastCustomer.Address,
                Email = lastCustomer.Email,
                PhoneNumber = lastCustomer.PhoneNumber,
                CreatedAt = lastCustomer.CreatedAt,
                UpdatedAt = lastCustomer.UpdatedAt
            };
        }


        public async Task<PaginatedListDto<CustomerDto>> GetCustomers(int userId,
            string? model, string? type, string? searchName,
            string? searchAdress, int page, int pageSize)
        {

            IQueryable<Customer> queryCustomers = _context.Customers.Where(x => x.UserId == userId && x.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                queryCustomers = queryCustomers.Where(t => t.Name.Contains(searchName));
            }
            if (!string.IsNullOrWhiteSpace(searchAdress))
            {
                queryCustomers = queryCustomers.Where(t => t.Address.Contains(searchAdress));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!string.IsNullOrWhiteSpace(model))
                {
                    if (type == "Desc")
                    {
                        switch (model)
                        {
                            case "Id": queryCustomers = queryCustomers.OrderByDescending(x => x.Id); break;
                            case "CreatedAt": queryCustomers = queryCustomers.OrderByDescending(x => x.CreatedAt); break;
                            case "UpdatedAt": queryCustomers = queryCustomers.OrderByDescending(x => x.UpdatedAt); break;
                            default: break;
                        }
                    }
                    if(type == "Asc")
                    {
                        switch (model)
                        {
                            case "Id": queryCustomers = queryCustomers.OrderBy(x => x.Id); break;
                            case "CreatedAt": queryCustomers = queryCustomers.OrderBy(x => x.CreatedAt); break;
                            case "UpdatedAt": queryCustomers = queryCustomers.OrderBy(x => x.UpdatedAt); break;
                            default: break;
                        }
                    }

                }
            }

            var totalCount = await queryCustomers.CountAsync();
            var items = await queryCustomers
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return new PaginatedListDto<CustomerDto>(
                items.Select(t => new CustomerDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Address = t.Address,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                }), new PaginationMeta(page, pageSize, totalCount)
            );
        }

        public async Task<Customer?> GetCustomerByEmail(int userId,string email)
        {
            var user = await _context.Users.Include(u => u.Customers).FirstOrDefaultAsync(u => u.Id == userId);

            var findCustomer = user?.Customers.FirstOrDefault(c => c.Email == email);

            return findCustomer;
        }
    }
}
