CustomerManagementApi.Client — Quick start
=========================================

Purpose
-------
A minimal .NET 8 console client that demonstrates usage of the CustomersController API:
- create a customer (POST /api/customers)
- list customers (GET /api/customers)
- get a customer by id (GET /api/customers/{id})
- delete a customer (DELETE /api/customers/{id})

Prerequisites
-------------
- .NET 8 SDK
- The API server (project CustomerManagementApi) running locally. By default this client targets:
  https://localhost:5001
  If your API uses a different URL/port, set environment variable API_BASE_URL or pass it as the first command-line argument.

Run
---
dotnet run --project CustomerManagementApi.Client

Or specify base URL:
dotnet run --project CustomerManagementApi.Client -- "http://localhost:5000"

Notes
-----
- The client uses System.Net.Http.Json and System.Text.Json.
- Replace the base URL if your API launch profile uses a different port or HTTP vs HTTPS.CustomerManagementApi.Client — Quick start
=========================================

Purpose
-------
A minimal .NET 8 console client that demonstrates usage of the CustomersController API:
- create a customer (POST /api/customers)
- list customers (GET /api/customers)
- get a customer by id (GET /api/customers/{id})
- delete a customer (DELETE /api/customers/{id})

Prerequisites
-------------
- .NET 8 SDK
- The API server (project CustomerManagementApi) running locally. By default this client targets:
  https://localhost:5001
  If your API uses a different URL/port, set environment variable API_BASE_URL or pass it as the first command-line argument.

Run
---
dotnet run --project CustomerManagementApi.Client

Or specify base URL:
dotnet run --project CustomerManagementApi.Client -- "http://localhost:5000"

Notes
-----
- The client uses System.Net.Http.Json and System.Text.Json.
- Replace the base URL if your API launch profile uses a different port or HTTP vs HTTPS.