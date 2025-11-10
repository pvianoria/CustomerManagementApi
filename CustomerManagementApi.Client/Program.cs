using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

const string defaultBaseUrl = "https://localhost:5001"; // adjust for your launch profile
var baseUrl = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("API_BASE_URL") ?? defaultBaseUrl;

using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

Console.WriteLine($"Using API base URL: {baseUrl}");
Console.WriteLine();

// Simple demo flow:
// 1. Create a customer
// 2. List all customers
// 3. Get the created customer by id
// 4. Delete the created customer
// 5. List all customers again

try
{
    var newCustomer = new CustomerCreate { FirstName = "Demo", LastName = "User", Email = "demo.user@example.com" };
    var created = await CreateCustomerAsync(newCustomer);
    if (created is not null)
    {
        Console.WriteLine("Created customer:");
        Console.WriteLine(JsonSerializer.Serialize(created, jsonOptions));
        Console.WriteLine();

        Console.WriteLine("All customers after create:");
        var all = await GetAllCustomersAsync();
        PrintCustomers(all);

        Console.WriteLine();
        Console.WriteLine($"Get customer by id = {created.Id}");
        var single = await GetCustomerByIdAsync(created.Id);
        Console.WriteLine(single is not null ? JsonSerializer.Serialize(single, jsonOptions) : "Not found");

        Console.WriteLine();
        Console.WriteLine($"Deleting customer id = {created.Id}");
        var deleted = await DeleteCustomerAsync(created.Id);
        Console.WriteLine(deleted ? "Deleted" : "Not found / failed");

        Console.WriteLine();
        Console.WriteLine("All customers after delete:");
        PrintCustomers(await GetAllCustomersAsync());
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex}");
}

async Task<List<Customer>?> GetAllCustomersAsync()
{
    var resp = await client.GetAsync("/api/customers");
    if (resp.StatusCode == HttpStatusCode.OK)
    {
        return await resp.Content.ReadFromJsonAsync<List<Customer>>(jsonOptions);
    }

    Console.Error.WriteLine($"GetAll failed: {resp.StatusCode}");
    return null;
}

async Task<Customer?> GetCustomerByIdAsync(int id)
{
    var resp = await client.GetAsync($"/api/customers/{id}");
    if (resp.StatusCode == HttpStatusCode.OK)
    {
        return await resp.Content.ReadFromJsonAsync<Customer>(jsonOptions);
    }

    if (resp.StatusCode == HttpStatusCode.NotFound) return null;

    Console.Error.WriteLine($"GetById failed: {resp.StatusCode}");
    return null;
}

async Task<Customer?> CreateCustomerAsync(CustomerCreate payload)
{
    var resp = await client.PostAsJsonAsync("/api/customers", payload, jsonOptions);
    if (resp.StatusCode == HttpStatusCode.Created)
    {
        return await resp.Content.ReadFromJsonAsync<Customer>(jsonOptions);
    }

    Console.Error.WriteLine($"Create failed: {resp.StatusCode}");
    var text = await resp.Content.ReadAsStringAsync();
    if (!string.IsNullOrWhiteSpace(text)) Console.Error.WriteLine(text);
    return null;
}

async Task<bool> DeleteCustomerAsync(int id)
{
    var resp = await client.DeleteAsync($"/api/customers/{id}");
    return resp.StatusCode == HttpStatusCode.NoContent;
}

void PrintCustomers(List<Customer>? list)
{
    if (list == null || list.Count == 0)
    {
        Console.WriteLine("(no customers)");
        return;
    }

    foreach (var c in list)
    {
        Console.WriteLine($"- [{c.Id}] {c.FirstName} {c.LastName} <{c.Email}> createdAt={c.CreatedAt:O}");
    }
}

// Lightweight DTOs matching server model shape
public sealed record Customer
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record CustomerCreate
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
}