using InvoiceApiFinal.Data;
using InvoiceApiFinal.DTOs.RowInvoice;
using InvoiceApiFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApiFinal.Services.RowServices
{
    public class RowService : IRowService
    {
        private readonly InvoiceDbContext _context;
        public RowService(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceRow?> AddRow(RowCreateRequest createRequest, Invoice invoice)
        {
            var row = new InvoiceRow()
            {
                InvoiceId = invoice.Id,
                Description = createRequest.Description,
                Quantity = createRequest.Quantity,
                Amount = createRequest.Amount,
                Sum = (createRequest.Quantity * createRequest.Amount)
            };

            var created = _context.InvoiceRows.Add(row).Entity;

            invoice.TotalSum += created.Sum;

            await _context.SaveChangesAsync();

            if(created is null)
                return null;

            return created;

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

        public async Task<InvoiceRow?> CheckRow(int invoiceId, int rowId)
        {
            var invoice = await _context.Invoices.Include(x => x.Rows)
                .FirstOrDefaultAsync(m => m.Id == invoiceId && m.IsDeleted == false);

            var findRow = invoice?.Rows.FirstOrDefault(c => c.Id == rowId);

            if(findRow is null)
                return null;

            return findRow;
        }

        public async Task<InvoiceRow?> DeleteRow(InvoiceRow invoiceRow, Invoice invoice)
        {
            var deleted = _context.InvoiceRows.Remove(invoiceRow).Entity;

            invoice.TotalSum -= deleted.Sum;
            await _context.SaveChangesAsync();

            if (deleted is null)
                return null;

            return deleted;
        }
    }
}
