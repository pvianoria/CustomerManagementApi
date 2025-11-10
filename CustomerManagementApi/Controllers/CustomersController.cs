using CustomerManagementApi.Data;
using CustomerManagementApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementApi.Controllers;

/// <summary>
/// API controller that provides basic CRUD operations for <see cref="Customer"/> entities.
/// Uses an injected <see cref="AppDbContext"/> so the provider can be configured by DI.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Constructor accepting the application's <see cref="AppDbContext"/> via DI.
    /// </summary>
    public CustomersController(AppDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Retrieves all customers.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _db.Customers.ToListAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Retrieves a single customer by identifier.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        return customer == null ? NotFound() : Ok(customer);
    }

    /// <summary>
    /// Creates a new customer record.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        if (customer == null) throw new ArgumentNullException(nameof(customer));
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Deletes a customer by identifier.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}