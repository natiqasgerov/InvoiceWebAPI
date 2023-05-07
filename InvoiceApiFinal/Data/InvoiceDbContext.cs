using InvoiceApiFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApiFinal.Data
{
    public class InvoiceDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();
        public InvoiceDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
