using CustomerManagementApi.Controllers;
using CustomerManagementApi.Data;
using CustomerManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerManagementApi.Tests
{
    /// <summary>
    /// Unit tests for <see cref="CustomersController"/>.
    /// Tests use the controller's public methods to exercise the in-memory store.
    /// Each test resets the store to provide isolation.
    /// </summary>
    public partial class CustomersControllerTests
    {
        private readonly CustomersController _controller;

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

        public CustomersControllerTests()
        {
            _controller = new CustomersController(CreateContext());
        }

        /// <summary>
        /// Remove any existing customers by enumerating all and deleting them.
        /// Ensures test isolation because the controller uses a shared in-memory root.
        /// </summary>
        private async Task ResetDatabaseAsync()
        {
            var getAllResult = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(getAllResult);
            var customers = Assert.IsType<List<Customer>>(ok.Value);
            // Delete each existing customer
            foreach (var c in customers.ToList())
            {
                var delResult = await _controller.Delete(c.Id);
                // Accept either NoContent (deleted) or NotFound (already gone)
                Assert.True(delResult is NoContentResult || delResult is NotFoundResult);
            }
        }

        [Fact(DisplayName = "GetAll initially returns empty list")]
        public async Task GetAll_InitiallyEmpty_ReturnsEmptyList()
        {
            await ResetDatabaseAsync();

            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Customer>>(ok.Value);
            Assert.Empty(list);
        }

        [Fact(DisplayName = "Create valid customer returns CreatedAtAction with assigned id and createdAt")]
        public async Task Create_ValidCustomer_ReturnsCreated()
        {
            await ResetDatabaseAsync();

            var payload = new Customer { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" };
            var result = await _controller.Create(payload);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);

            var createdCustomer = Assert.IsType<Customer>(created.Value!);
            Assert.True(createdCustomer.Id > 0, "Created customer must have an assigned Id.");
            Assert.Equal(payload.FirstName, createdCustomer.FirstName);
            Assert.Equal(payload.LastName, createdCustomer.LastName);
            Assert.Equal(payload.Email, createdCustomer.Email);

            // CreatedAt should be set (default on model). Check it's reasonably recent.
            var age = DateTime.UtcNow - createdCustomer.CreatedAt;
            Assert.True(age < TimeSpan.FromMinutes(1), "CreatedAt should be recent (UtcNow).");
        }

        [Fact(DisplayName = "GetById for existing customer returns OK with customer")]
        public async Task GetById_Existing_ReturnsCustomer()
        {
            await ResetDatabaseAsync();

            var payload = new Customer { FirstName = "Bob", LastName = "Jones", Email = "bob@example.com" };
            var createResult = await _controller.Create(payload);
            var created = Assert.IsType<CreatedAtActionResult>(createResult);
            var createdCustomer = Assert.IsType<Customer>(created.Value!);

            var getResult = await _controller.GetById(createdCustomer.Id);
            var ok = Assert.IsType<OkObjectResult>(getResult);
            var fetched = Assert.IsType<Customer>(ok.Value!);
            Assert.Equal(createdCustomer.Id, fetched.Id);
            Assert.Equal(createdCustomer.Email, fetched.Email);
        }

        [Fact(DisplayName = "GetById non-existing returns NotFound")]
        public async Task GetById_NonExisting_ReturnsNotFound()
        {
            await ResetDatabaseAsync();

            var result = await _controller.GetById(999_999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Delete existing customer returns NoContent and resource is removed")]
        public async Task Delete_Existing_ReturnsNoContent()
        {
            await ResetDatabaseAsync();

            var payload = new Customer { FirstName = "Carol", LastName = "White", Email = "carol@example.com" };
            var createResult = await _controller.Create(payload);
            var created = Assert.IsType<CreatedAtActionResult>(createResult);
            var createdCustomer = Assert.IsType<Customer>(created.Value!);

            var deleteResult = await _controller.Delete(createdCustomer.Id);
            Assert.IsType<NoContentResult>(deleteResult);

            // Verify it has been removed
            var getResult = await _controller.GetById(createdCustomer.Id);
            Assert.IsType<NotFoundResult>(getResult);
        }

        [Fact(DisplayName = "Delete non-existing returns NotFound")]
        public async Task Delete_NonExisting_ReturnsNotFound()
        {
            await ResetDatabaseAsync();

            var result = await _controller.Delete(42_424_242);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Create null customer throws ArgumentNullException")]
        public async Task Create_NullCustomer_ThrowsArgumentNullException()
        {
            await ResetDatabaseAsync();

            // Intentionally pass null to exercise controller behavior when model is missing.
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _controller.Create(null!));
        }

        [Fact(DisplayName = "Multiple creates assign distinct incremental Ids")]
        public async Task Create_Multiple_AssignsDistinctIds()
        {
            await ResetDatabaseAsync();

            var first = new Customer { FirstName = "First" };
            var second = new Customer { FirstName = "Second" };

            var r1 = Assert.IsType<CreatedAtActionResult>(await _controller.Create(first));
            var c1 = Assert.IsType<Customer>(r1.Value!);

            var r2 = Assert.IsType<CreatedAtActionResult>(await _controller.Create(second));
            var c2 = Assert.IsType<Customer>(r2.Value!);

            Assert.NotEqual(c1.Id, c2.Id);
            Assert.True(c2.Id > c1.Id, "Second created Id should be greater than first when using the in-memory provider's generation behavior.");
        }
    }
}