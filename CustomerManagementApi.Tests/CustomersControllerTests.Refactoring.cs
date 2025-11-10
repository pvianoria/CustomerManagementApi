using CustomerManagementApi.Controllers;
using CustomerManagementApi.Data;
using CustomerManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementApi.Tests;

public partial class CustomersControllerTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task Create_And_GetById_Works()
    {
        using var ctx = CreateInMemoryContext(Guid.NewGuid().ToString());
        var controller = new CustomersController(ctx);

        var payload = new Customer { FirstName = "Test" };
        var createResult = await controller.Create(payload);
        var created = Assert.IsType<CreatedAtActionResult>(createResult);
        var createdCustomer = Assert.IsType<Customer>(created.Value!);

        var getResult = await controller.GetById(createdCustomer.Id);
        var ok = Assert.IsType<OkObjectResult>(getResult);
        var fetched = Assert.IsType<Customer>(ok.Value!);
        Assert.Equal(createdCustomer.Id, fetched.Id);
    }
}