using CustomerManagementApi.Data;
using CustomerManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerManagementApi.Controllers
{
    /// <summary>
    /// API controller that provides basic CRUD operations for <see cref="Customer"/> entities.
    /// Uses an in-memory EF Core store for demonstration and testing purposes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        /// <summary>
        /// Shared in-memory database root so multiple <see cref="AppDbContext"/> instances
        /// operate on the same in-memory store for the lifetime of the process.
        /// </summary>
        private static readonly InMemoryDatabaseRoot _dbRoot = new();

        /// <summary>
        /// Creates a new <see cref="AppDbContext"/> configured to use the in-memory provider.
        /// </summary>
        /// <returns>A configured <see cref="AppDbContext"/> instance.</returns>
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("CustomersDb", _dbRoot)
                .Options;

            return new AppDbContext(options);
        }

        /// <summary>
        /// Retrieves all customers.
        /// </summary>
        /// <returns>200 OK with the list of customers.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using var ctx = CreateContext();
            var customers = await ctx.Customers.ToListAsync();
            return Ok(customers);
        }

        /// <summary>
        /// Retrieves a single customer by identifier.
        /// </summary>
        /// <param name="id">The customer identifier.</param>
        /// <returns>200 OK with the customer when found, otherwise 404 NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var ctx = CreateContext();
            var customer = await ctx.Customers.FindAsync(id);
            return customer == null ? NotFound() : Ok(customer);
        }

        /// <summary>
        /// Creates a new customer record.
        /// </summary>
        /// <param name="customer">The customer payload to create.</param>
        /// <returns>201 Created with a Location header pointing to the new resource.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            using var ctx = CreateContext();
            ctx.Customers.Add(customer);
            await ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        /// <summary>
        /// Deletes a customer by identifier.
        /// </summary>
        /// <param name="id">The customer identifier to remove.</param>
        /// <returns>204 NoContent when successful, 404 NotFound if the customer does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var ctx = CreateContext();
            var customer = await ctx.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            ctx.Customers.Remove(customer);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}