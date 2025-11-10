// Program.cs - application startup for CustomerManagementApi
// Configures services, middleware, and runs the web application.
//
// Notes:
// - This file uses top-level statements (minimal hosting model).
// - Register long-lived services (for example `AppDbContext`) with the DI container here.
// - Keep startup changes small for demo apps; for larger apps consider a dedicated Startup class.

using CustomerManagementApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext provider based on configuration.
// appsettings.json keys:
//   "Database": { "Provider": "InMemory" | "Sqlite", "ConnectionString": "Data Source=..." }
var dbSection = builder.Configuration.GetSection("Database");
var provider = dbSection.GetValue<string>("Provider", "InMemory");

if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
{
    var conn = dbSection.GetValue<string>("ConnectionString") ??
               builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));
}
else
{
    // InMemory provider (use a fixed name for process lifetime; tests should create their own contexts)
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("CustomersDb"));
}

// Register framework services:
// - MVC controllers for the API surface
// - OpenAPI/Swagger generation for development and testing
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// In development enable Swagger UI to explore the API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Map attribute-routed controllers to endpoints.
app.MapControllers();

// Start the web application and block the calling thread until shutdown.
app.Run();