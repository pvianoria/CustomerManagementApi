CustomersController — README
============================

Overview
--------
`CustomersController` is a small ASP.NET Core Web API controller that provides basic CRUD operations for `Customer` entities using an in-memory EF Core database. It is intended for demos, tests, or small in-memory scenarios — not production use.

Key implementation details
--------------------------
- Each action creates a new `AppDbContext` instance via the static helper `CreateContext()`.
- `CreateContext()` configures EF Core to use an in-memory database named "CustomersDb" and shares a single `InMemoryDatabaseRoot` (`_dbRoot`) so data persists across `DbContext` instances while the process runs.
- `using` blocks ensure each `DbContext` is properly disposed.
- No authentication, input validation, or paging is implemented in this controller.

Exposed endpoints
-----------------
- GET `/api/customers`
  - Returns 200 OK with the list of all customers.
  - Example:
    curl -sS http://localhost:5000/api/customers

- GET `/api/customers/{id}`
  - Returns 200 OK with the `Customer` when found, otherwise 404 NotFound.
  - Example:
    curl -sS http://localhost:5000/api/customers/1

- POST `/api/customers`
  - Accepts a `Customer` JSON body, adds it to the database and returns 201 Created with a Location header pointing to the new resource (via `CreatedAtAction`).
  - Example:
    curl -sS -H "Content-Type: application/json" -d '{"firstName":"Jane","lastName":"Doe","email":"jane@example.com"}' http://localhost:5000/api/customers

- DELETE `/api/customers/{id}`
  - Removes the specified customer if found and returns 204 NoContent. Returns 404 if the customer does not exist.
  - Example:
    curl -sS -X DELETE http://localhost:5000/api/customers/1

Customer model (summary)
------------------------
The application operates on a `Customer` entity. Expected properties:
- `Id` (int) — primary key.
- `FirstName` (string?) — given name.
- `LastName` (string?) — family name.
- `Email` (string?) — contact email.
- `CreatedAt` (DateTime) — timestamp when the record was created.

Recommendations for the `Customer` model:
- Set `CreatedAt` on creation (for example via constructor or default value) and consider using `DateTime.UtcNow`.
- Add validation attributes (e.g., `[Required]`, `[EmailAddress]`, length limits) or use a DTO to validate/shape incoming requests.
- Consider immutability for audit properties or make them read-only from the API surface.

AppDbContext
-----------
- Located in `CustomerManagementApi.Data.AppDbContext`.
- Inherits from `DbContext` and exposes a `DbSet<Customer>`:
  - `public DbSet<Customer> Customers => Set<Customer>();`
- Purpose: encapsulates EF Core access to the `Customer` table/set.
- Current usage: created directly by the controller using `DbContextOptionsBuilder` configured with the in-memory provider and a shared `InMemoryDatabaseRoot`.
- Recommendations:
  - Register `AppDbContext` with the DI container in `Program.cs` (for example `builder.Services.AddDbContext<AppDbContext>(options => ...)`) and inject it into controllers. This simplifies lifetime management and makes it straightforward to swap providers (in-memory → SQL Server, PostgreSQL, etc.).
  - Add database configuration/connection strings to `appsettings.json` for non-demo scenarios.
  - Consider applying migrations when using a relational provider.

Program (application startup)
----------------------------
- Located at `CustomerManagementApi.Program`.
- Current responsibilities:
  - Builds a minimal host with `WebApplication.CreateBuilder(args)`.
  - Registers MVC controllers, Swagger/OpenAPI, and middleware.
  - In development, enables Swagger UI.
  - Enables HTTPS redirection and maps controllers before calling `app.Run()`.
- Typical code flow:
  - `builder.Services.AddControllers();`
  - `builder.Services.AddEndpointsApiExplorer();`
  - `builder.Services.AddSwaggerGen();`
  - `var app = builder.Build();`
  - `app.UseSwagger(); app.UseSwaggerUI();` (dev only)
  - `app.UseHttpsRedirection(); app.MapControllers(); app.Run();`
- Recommendations:
  - Register `AppDbContext` with `builder.Services` and configure provider there.
  - Add logging, exception handling middleware (e.g., `app.UseExceptionHandler`), and authorization as needed.
  - Externalize environment-specific settings using `appsettings.{Environment}.json`.

Notes, limitations & recommendations
------------------------------------
- The in-memory provider is non-persistent between application restarts and is suitable only for testing or demos.
- There is no validation or error handling for malformed input; add model validation attributes or manual checks.
- Consider adding: PUT/PATCH for updates, DTOs to decouple API surface from EF entities, paging for `GetAll`, logging, and switching to a real database for production.
- For multi-threaded or multi-instance scenarios, replace the in-memory store with a real database; the current `_dbRoot` only preserves in-memory state within a single process.

Contact / maintenance
---------------------
This file documents `CustomerManagementApi\Controllers\CustomersController.cs`, `CustomerManagementApi\Data\AppDbContext.cs`, the `Customer` model, and `CustomerManagementApi\Program.cs`. Update this README when the persistence approach, model shape, or startup configuration changes.