// Program.cs - application startup for CustomerManagementApi
// Configures services, middleware, and runs the web application.
//
// Notes:
// - This file uses top-level statements (minimal hosting model).
// - Register long-lived services (for example `AppDbContext`) with the DI container here.
// - Keep startup changes small for demo apps; for larger apps consider a dedicated Startup class.

var builder = WebApplication.CreateBuilder(args);

// Register framework services:
// - MVC controllers for the API surface
// - OpenAPI/Swagger generation for development and testing
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: For production or more realistic scenarios, register `AppDbContext` here:
// builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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