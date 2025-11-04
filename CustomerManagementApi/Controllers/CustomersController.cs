using CustomerManagementApi.Data;
using CustomerManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private static readonly InMemoryDatabaseRoot _dbRoot = new();

        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("CustomersDb", _dbRoot)
                .Options;

            return new AppDbContext(options);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using var ctx = CreateContext();
            var customers = await ctx.Customers.ToListAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var ctx = CreateContext();
            var customer = await ctx.Customers.FindAsync(id);
            return customer == null ? NotFound() : Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            using var ctx = CreateContext();
            ctx.Customers.Add(customer);
            await ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

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