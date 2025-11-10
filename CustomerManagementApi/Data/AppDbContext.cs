using Microsoft.EntityFrameworkCore;
using CustomerManagementApi.Models;

namespace CustomerManagementApi.Data
{
    /// <summary>
    /// EF Core database context for the application. Exposes entity sets used by the API.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructs a new <see cref="AppDbContext"/> using the provided options.
        /// </summary>
        /// <param name="options">The options to configure the context (provider, connection string, etc.).</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Gets the set of <see cref="Customer"/> entities.
        /// </summary>
        public DbSet<Customer> Customers => Set<Customer>();
    }
}