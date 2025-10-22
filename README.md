# Motorcycle Rental System

A complete .NET 8 backend application for managing motorcycles, couriers (delivery drivers), and rentals built with Clean Architecture principles.

## 🏗️ Architecture

The solution follows **Clean Architecture** with clear separation of concerns:

- **Domain**: Core business entities and interfaces
- **Application**: Business logic, CQRS commands/queries, DTOs, validators
- **Infrastructure**: Data access (EF Core + PostgreSQL), RabbitMQ, file storage
- **Api**: REST API controllers, middleware, configuration

## 🛠️ Technologies

- **.NET 8** (C# 12)
- **PostgreSQL** - Main database
- **Entity Framework Core** - ORM
- **RabbitMQ** - Message broker
- **MediatR** - CQRS pattern
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Unit and integration testing

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (for PostgreSQL and RabbitMQ)
- [PostgreSQL](https://www.postgresql.org/download/) (or use Docker)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (or use Docker)

## 🚀 Getting Started

### 1. Navigate to the project directory

```bash
cd Desafio-BackEnd
```

### 2. Start dependencies with Docker

```bash
docker-compose up -d
```

This will start:
- PostgreSQL on port 5432
- RabbitMQ on port 5672 (management UI on port 15672)

### 3. Update database connection string

Edit `src/Api/appsettings.Development.json` and update the connection string if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=MotorcycleRentalDb;Username=postgres;Password=postgres"
}
```

### 4. Run database migrations

```bash
cd src/Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Api
dotnet ef database update --startup-project ../Api
```

### 5. Run the application

```bash
cd src/Api
dotnet run
```

The API will be available at:
- HTTPS: https://localhost:7001
- HTTP: http://localhost:5001
- Swagger UI: https://localhost:7001/swagger

### 6. Run tests

```bash
# Unit tests
dotnet test tests/UnitTests

# Integration tests
dotnet test tests/IntegrationTests

# All tests
dotnet test
```

## 📚 API Endpoints

### Motorcycles
- `POST /api/motorcycles` - Register a new motorcycle
- `GET /api/motorcycles` - List all motorcycles (optional filter by license plate)
- `GET /api/motorcycles/{id}` - Get motorcycle by ID
- `PUT /api/motorcycles/{id}/license-plate` - Update license plate
- `DELETE /api/motorcycles/{id}` - Delete motorcycle (only if no rental history)

### Couriers
- `POST /api/couriers` - Register a new courier
- `POST /api/couriers/{id}/driver-license-image` - Upload/update driver license image (PNG/BMP only)

### Rentals
- `POST /api/rentals` - Create a new rental
- `GET /api/rentals/{id}` - Get rental by ID
- `PUT /api/rentals/{id}/return` - Return a rented motorcycle

## 🔑 Business Rules

### Motorcycles
- License plate must be unique
- On registration, publishes "MotorcycleRegistered" event to RabbitMQ
- If motorcycle year is 2024, the event is persisted in the database
- Cannot be deleted if it has rental history

### Couriers
- CNPJ must be unique (14 characters)
- Driver license number must be unique
- Driver license type must be A, B, or A+B
- Driver license image must be PNG or BMP format
- Image stored in local file system (not in database)

### Rentals
- Rental plans: 7, 15, 30, 45, or 50 days
- Daily costs:
  - 7 days: R$30.00
  - 15 days: R$28.00
  - 30 days: R$22.00
  - 45 days: R$20.00
  - 50 days: R$18.00
- Start date is always one day after creation
- Only couriers with license type A or A+B can rent motorcycles
- **Early return penalties:**
  - 7-day plan: 20% on unused days
  - 15-day plan: 40% on unused days
- **Late return fee:** R$50.00 per extra day

## 📁 Project Structure

```
MotorcycleRentalSystem/
├── src/
│   ├── Api/                      # REST API controllers and configuration
│   ├── Application/              # Business logic, CQRS, DTOs, validators
│   ├── Domain/                   # Core entities, enums, interfaces
│   └── Infrastructure/           # Data access, messaging, file storage
├── tests/
│   ├── UnitTests/                # Unit tests
│   └── IntegrationTests/         # Integration tests
├── docker-compose.yml            # Docker services configuration
└── README.md
```

## 🔧 Configuration

### Database
Configure in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=MotorcycleRentalDb;Username=postgres;Password=postgres"
}
```

### RabbitMQ
Configure in `appsettings.json`:
```json
"RabbitMQ": {
  "Host": "localhost",
  "Port": "5672",
  "Username": "guest",
  "Password": "guest"
}
```

### File Storage
Configure in `appsettings.json`:
```json
"FileStorage": {
  "Path": "uploads"
}
```

## 🧪 Testing

The solution includes comprehensive test coverage:

### Unit Tests
- Motorcycle CQRS operations
- Courier registration validation
- Rental cost calculation
- Domain entity logic

### Integration Tests
- Full API workflow
- Database operations
- Message publishing
- File upload

## 📝 Logging

Logs are written to:
- Console (development)
- File: `logs/log-{Date}.txt`

Configure logging in `appsettings.json` using Serilog.

## 🐳 Docker Support

The `docker-compose.yml` provides:
- PostgreSQL 15
- RabbitMQ 3 (with management plugin)

Access RabbitMQ Management UI at: http://localhost:15672
- Username: guest
- Password: guest

## 📖 API Documentation

Full API documentation is available via Swagger UI when running the application:
- https://localhost:7001/swagger

## ⚠️ Important Notes

- All code is in **English**
- Follows **RESTful API conventions**
- Uses **Clean Architecture** principles
- Implements **CQRS pattern** with MediatR
- Global exception handling middleware
- Structured logging with Serilog
- Input validation with FluentValidation
- No company names in code or documentation

## 🤝 Contributing

This is a technical assessment project. For production use, consider adding:
- Authentication and authorization
- Rate limiting
- Caching
- Distributed tracing
- Health checks
- API versioning
- CORS configuration
- More comprehensive error handling

## 📄 License

This project is for educational and assessment purposes.
