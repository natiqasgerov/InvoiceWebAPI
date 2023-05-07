using InvoiceApiFinal.DTOs.Customer;
using InvoiceApiFinal.DTOs.Pagination;
using InvoiceApiFinal.Models;

namespace InvoiceApiFinal.Services.CustomerServices
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByEmail(int userId,string email);
        Task<CustomerDto> CreateCustomer(int userId,CreateCustomerForm form);
        Task<Customer?> GetCustomerById(int userId,int customerId);
        Task<CustomerDto?> UpdateCustomer(int userId,int customerId, CustomerEditRequest editRequest);
        Task<Customer?> DeleteCustomerById(int userId,int customerId);
        Task<Customer?> ArchiveCustomer(int userId,int customerId);

        Task<PaginatedListDto<CustomerDto>> GetCustomers(int userId,string model,
            string type,string searchName,string searchAdress, int page,int pageSize);

    }
}
