# Book Management API

A RESTful API for managing books built with ASP.NET Core and Entity Framework Core.

## Features

- Complete CRUD operations for books
- Bulk operations support (create/delete)
- JWT Authentication
- Book popularity tracking
- Swagger documentation
- Entity Framework Core with SQL Server

## Prerequisites

- .NET 7.0 or later
- SQL Server
- Visual Studio 2022 or VS Code

## Getting Started

1. Clone the repository
```bash
git clone https://github.com/yourusername/book-management-api.git
cd book-management-api
```

2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  },
  "Jwt": {
    "Key": "Your_Secret_Key_Here_Min_16_Characters",
    "Issuer": "your-domain.com",
    "Audience": "your-domain.com"
  }
}
```

3. Run Entity Framework migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

## API Endpoints

### Authentication
- POST /api/auth/register - Register new user
- POST /api/auth/login - Login user

### Books
- GET /api/books - Get all books
- GET /api/books/{id} - Get specific book
- POST /api/books - Add single book
- POST /api/books/bulk - Add multiple books
- PUT /api/books/{id} - Update book
- DELETE /api/books/{id} - Soft delete single book
- DELETE /api/books - Bulk soft delete books
- GET /api/deleted - Get soft deleted books
- POST /api/{id}/restore - Restore soft deleted books
- GET /api/books/popular - Get popular books

## Authentication

The API uses JWT Bearer authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer your-token-here
```

## Development

1. Install dependencies:
```bash
dotnet restore
```

2. Run tests:
```bash
dotnet test
```

3. Build the project:
```bash
dotnet build
```
