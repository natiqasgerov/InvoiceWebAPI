using InvoiceApiFinal.DTOs.Customer;
using InvoiceApiFinal.DTOs.Filtering;
using InvoiceApiFinal.DTOs.Pagination;
using InvoiceApiFinal.DTOs.Sorting.CustomerSortings;
using InvoiceApiFinal.Providers;
using InvoiceApiFinal.Services.CustomerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InvoiceApiFinal.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    private readonly IUserProvider _userProvider;
    public CustomerController(ICustomerService customerService, IUserProvider userProvider)
    {
        _customerService = customerService;
        _userProvider = userProvider;
    }

    /// <summary>
    /// Creates a new customer using the provided form data.
    /// </summary>
    /// <param name="createForm">The data to be used for the create operation.</param>
    /// <returns>
    /// Returns an ActionResult object containing a CustomerDto object if the customer is created successfully,
    /// or an appropriate error message if the create operation fails.
    /// </returns>

    [HttpPost("Create")]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerForm createForm)
    {
        var user = _userProvider.GetUserInfo();
        if (user is null)
        {
            Log.Warning("Unauthorized access for customer creation.");
            return Unauthorized();
        }

        var check = await _customerService.GetCustomerByEmail(user.Id, createForm.Email);

        if (check is not null)
        {
            Log.Warning("Conflict while creating customer. Customer with email {Email} already exists.", createForm.Email);
            return Conflict();
        }


        var addedUser = await _customerService.CreateCustomer(user.Id, createForm);

        if (addedUser is not null)
        {
            Log.Information("Customer created successfully. CustomerId: {CustomerId}", addedUser.Id);
            return addedUser;
        }

        Log.Error("Error occurred while creating customer.");
        return BadRequest();
    }


    /// <summary>
    /// Edits the details of an existing customer with the specified customer ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to edit.</param>
    /// <param name="editRequest">The object containing the updated details for the customer.</param>
    /// <returns>The updated customer object if successful; otherwise, an appropriate error response.</returns>

    [HttpPut("{customerId}/Edit")]
    public async Task<ActionResult<CustomerDto>> EditCustomer(int customerId,[FromBody] CustomerEditRequest editRequest)
    {
        var user = _userProvider.GetUserInfo();
        if (user is null)
        {
            Log.Warning("Unauthorized access for customer edit.");
            return Unauthorized();
        }

        var checkCustomer = await _customerService.GetCustomerById(user.Id, customerId);

        if (checkCustomer is null) {

            Log.Warning("Customer not found for edit. CustomerId: {CustomerId}", customerId);
            return NotFound();
        }


        if (!string.IsNullOrWhiteSpace(editRequest.Email))
        {
            var checkEmail = await _customerService.GetCustomerByEmail(user.Id, editRequest.Email);
            if (checkEmail is not null)
            {
                Log.Warning("Conflict while editing customer. Customer with email {Email} already exists.", editRequest.Email);
                return Conflict();
            }
        }

        var updated = await _customerService.UpdateCustomer(user.Id,customerId, editRequest);

        if (updated is null)
        {
            Log.Error("Error occurred while editing customer.");
            return NotFound();
        }

        Log.Information("Customer edited successfully. CustomerId: {CustomerId}", customerId);
        return updated;
        
    }


    /// <summary>
    /// Deletes a customer with the specified customer ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to delete.</param>
    /// <returns>An ActionResult with the deleted customer DTO, or a NotFound result if the customer does not exist.</returns>

    [HttpDelete("{customerId}/Delete")]
    public async Task<ActionResult<CustomerDto>> DeleteCustomer(int customerId)
    {
        var user = _userProvider.GetUserInfo();
        if (user is null)
        {
            Log.Warning("Unauthorized access for customer deletion.");
            return Unauthorized();
        }

        var deleted = await _customerService.DeleteCustomerById(user.Id, customerId);

        if (deleted is null)
        {
            Log.Warning("Customer not found for deletion. CustomerId: {CustomerId}", customerId);
            return NotFound();
        }

        Log.Information("Customer deleted successfully. CustomerId: {CustomerId}", customerId);
        return new CustomerDto
        {
            Id = deleted.Id,
            Name = deleted.Name,
            Address = deleted.Address,
            Email = deleted.Email,
            PhoneNumber = deleted.PhoneNumber,
            CreatedAt = deleted.CreatedAt,
            UpdatedAt = deleted.UpdatedAt,
        };
    }


    /// <summary>
    /// Archives the customer with the given customer ID.
    /// </summary>
    /// <param name="customerId">The ID of the customer to archive.</param>
    /// <returns>The archived customer DTO if successful, or a not found error if the customer doesn't exist.</returns>
    
    [HttpDelete("{customerId}/Archive")]
    public async Task<ActionResult<ArchiveCustomerDto>> ArchiveCustomer(int customerId)
    {
        var user = _userProvider.GetUserInfo();
        if (user is null)
        {
            Log.Warning("Unauthorized access for customer archiving.");
            return Unauthorized();
        }

        var archived = await _customerService.ArchiveCustomer(user.Id, customerId);

        if (archived is null)
        {
            Log.Warning("Customer not found for archiving. CustomerId: {CustomerId}", customerId);
            return NotFound();
        }

        Log.Information("Customer archived successfully. CustomerId: {CustomerId}", customerId);
        return new ArchiveCustomerDto
        {
            Id = archived.Id,
            Name = archived.Name,
            Address = archived.Address,
            Email = archived.Email,
            PhoneNumber = archived.PhoneNumber,
            CreatedAt = archived.CreatedAt,
            UpdatedAt = archived.UpdatedAt,
            DeletedAt = archived.DeletedAt,
            IsDeleted = archived.IsDeleted
        };
    }


    /// <summary>
    /// Retrieves a paginated list of customers.
    /// </summary>
    /// <param name="sortingRequest">The sorting parameters for the customer list.</param>
    /// <param name="filterRequest">The filtering parameters for the customer list.</param>
    /// <param name="paginationRequest">The pagination parameters for the customer list.</param>
    /// <returns>A paginated list of customer data transfer objects.</returns>
    
    [HttpGet("GetList/filtering/sorting")]
    public async Task<ActionResult<PaginatedListDto<CustomerDto>>> GetCustomerList([FromQuery] CustomerSortingRequest sortingRequest,
        [FromQuery] CustomerFilterRequest filterRequest, [FromQuery] PaginationRequest paginationRequest)
    {
        var user = _userProvider.GetUserInfo();
        if (user is null)
        {
            Log.Warning("Unauthorized access for customer getting customer list.");
            return Unauthorized();
        }

        return await _customerService.GetCustomers(user.Id,
            sortingRequest.model.ToString(),
            sortingRequest.type.ToString(),
            filterRequest.Name,
            filterRequest.Address,
            paginationRequest.Page,
            paginationRequest.PageSize);

    }


    /// <summary>
    /// Returns the customer with the given customerId.
    /// </summary>
    /// <param name="customerId">The ID of the customer to retrieve.</param>
    /// <returns>The customer with the given ID, or NotFound if no such customer exists.</returns>

    [HttpGet("{customerId}/GetCustomer")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int customerId)
    {
        var user = _userProvider?.GetUserInfo();
        if (user is null)
        {
            Log.Warning("Unauthorized access for getting customer.");
            return Unauthorized();
        }

        var get = await _customerService.GetCustomerById(user.Id,customerId);

        if (get is null)
        {
            Log.Warning("Customer not found. CustomerId: {CustomerId}", customerId);
            return NotFound();
        }

        Log.Information("Customer retrieved successfully. CustomerId: {CustomerId}", customerId);
        return new CustomerDto
        {
            Id = get.Id,
            Name = get.Name,
            Address = get.Address,
            Email = get.Email,
            PhoneNumber = get.PhoneNumber,
            CreatedAt = get.CreatedAt,
            UpdatedAt = get.UpdatedAt,
        };
    }
}
