using InvoiceApiFinal.Data;
using InvoiceApiFinal.DTOs.Customer;
using InvoiceApiFinal.DTOs.Invoice;
using InvoiceApiFinal.DTOs.Pagination;
using InvoiceApiFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApiFinal.Services.InvoiceServices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly InvoiceDbContext _context;
        public InvoiceService(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> ArchiveInvoice(Invoice invoice)
        {
            invoice.DeletedAt = new DateTimeOffset(DateTime.Now);
            invoice.IsDeleted = true;
            invoice.UpdatedAt = new DateTimeOffset(DateTime.Now);

            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task<Invoice?> ChangeInvoiceStatus(Invoice invoice, string status)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                invoice.Status = status;
                switch (status)
                {
                    case "Received":
                        invoice.StartDate = new DateTimeOffset(DateTime.Now); break;
                    case "Paid":
                        invoice.EndDate = new DateTimeOffset(DateTime.Now); break;
                    default:
                        break;
                }
            }
            invoice.UpdatedAt = new DateTimeOffset(DateTime.Now);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Customer?> CheckCustomer(int userId, int customerId)
        {
            var user = await _context.Users.Include(u => u.Customers).FirstOrDefaultAsync(u => u.Id == userId);

            var findCustomer = user?.Customers.FirstOrDefault(c => c.Id == customerId && c.IsDeleted == false);

            if (findCustomer is null)
                return null;
            return findCustomer;
        }

        public async Task<Invoice?> CheckInvoice(int customerId, int invoiceId)
        {
            var customer = await _context.Customers.Include(u => u.Invoices)
                .FirstOrDefaultAsync(u => u.Id == customerId && u.IsDeleted == false);

            var findInvoice = await _context.Invoices.Include(u => u.Rows)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.IsDeleted == false);
          

            if (findInvoice is null) return null;

            return findInvoice;
        }

        public async Task<Invoice?> CreateInvoice(Customer customer, InvoiceCreateRequest createRequest)
        {
            var invoice = new Invoice
            {
                CustomerId = customer.Id,
                Title = createRequest.Title,
                Comment = createRequest.Comment,
                Status = "Created",
                CreatedAt = new DateTimeOffset(DateTime.Now),
                UpdatedAt = new DateTimeOffset(DateTime.Now)
            };

            invoice = _context.Invoices.Add(invoice).Entity;

            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task<Invoice?> DeleteInvoice(Invoice invoice)
        {
            var deletedInvoice = _context.Remove(invoice).Entity;
            await _context.SaveChangesAsync();

            if(deletedInvoice is null)
                return null;

            return deletedInvoice;
        }

        public async Task<PaginatedListDto<InvoiceInfoDto>> GetInvoices(int customerId,
            string? model, string? type,
            string? title,
            int page, int pageSize)
        {

            var customer = await _context.Customers.FirstAsync(x => x.Id == customerId);


            IQueryable<Invoice> queryInvoices = _context.Invoices.Include(x => x.Rows).Where(x => x.CustomerId == customerId && x.IsDeleted == false);


            if (!string.IsNullOrWhiteSpace(title))
            {
                queryInvoices = queryInvoices.Where(t => t.Title.Contains(title));
            }

            if(!string.IsNullOrWhiteSpace(type))
            {
                if (!string.IsNullOrWhiteSpace(model))
                {
                    if(type == "Desc")
                    {
                        switch (model)
                        {
                            case "Id": queryInvoices = queryInvoices.OrderByDescending(x => x.Id);break;
                            case "TotalSum": queryInvoices = queryInvoices.OrderByDescending(x => x.TotalSum);break;
                            case "StartDate": queryInvoices = queryInvoices.OrderByDescending(x => x.StartDate);break;
                            case "EndDate": queryInvoices = queryInvoices.OrderByDescending(x => x.EndDate);break;
                            case "CreatedAt": queryInvoices = queryInvoices.OrderByDescending(x => x.CreatedAt);break;
                            case "UpdatedAt": queryInvoices = queryInvoices.OrderByDescending(x => x.UpdatedAt);break;
                            default:
                                break;
                        }
                    }

                    if (type == "Asc")
                    {
                        switch (model)
                        {
                            case "Id": queryInvoices = queryInvoices.OrderBy(x => x.Id); break;
                            case "TotalSum": queryInvoices = queryInvoices.OrderBy(x => x.TotalSum); break;
                            case "StartDate": queryInvoices = queryInvoices.OrderBy(x => x.StartDate); break;
                            case "EndDate": queryInvoices = queryInvoices.OrderBy(x => x.EndDate); break;
                            case "CreatedAt": queryInvoices = queryInvoices.OrderBy(x => x.CreatedAt); break;
                            case "UpdatedAt": queryInvoices = queryInvoices.OrderBy(x => x.UpdatedAt); break;
                            default:
                                break;
                        }
                    }
                }
            }

            var totalCount = await queryInvoices.CountAsync();
            var items = await queryInvoices.
                Skip((page - 1) * pageSize).
                Take(pageSize).ToListAsync();

            return new PaginatedListDto<InvoiceInfoDto>(
                items.Select(t => new InvoiceInfoDto
                {
                    CustomerName = customer.Name,
                    CustomerEmail = customer.Email,
                    InvoiceId = t.Id,
                    Comment = t.Comment,
                    Title = t.Title,
                    Status = t.Status,
                    TotalSum = t.TotalSum,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    InvoiceRows = t.Rows

                }), new PaginationMeta(page, pageSize, totalCount)
            );
        }

        public async Task<Invoice?> UpdateInvoice(Invoice invoice, InvoiceEditRequest editRequest)
        {
            if (!string.IsNullOrWhiteSpace(editRequest.Title))
            {
                invoice.Title = editRequest.Title;
            }
            if (!string.IsNullOrWhiteSpace(editRequest.Comment))
            {
                invoice.Comment = editRequest.Comment;
            }
            invoice.UpdatedAt = new DateTimeOffset(DateTime.Now);

            await _context.SaveChangesAsync();
            return invoice;
        }


        
    }
}
