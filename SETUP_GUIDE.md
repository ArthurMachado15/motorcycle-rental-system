# Setup Guide - Motorcycle Rental System

The complete .NET 8 backend application has been initialized with all required features.

## 📂 Solution Structure

```
MotorcycleRentalSystem/
├── src/
│   ├── Domain/                    # Domain entities, enums, interfaces
│   ├── Application/               # CQRS, DTOs, validators, business logic
│   ├── Infrastructure/            # EF Core, PostgreSQL, RabbitMQ, file storage
│   └── Api/                       # REST API controllers, middleware
├── tests/
│   ├── UnitTests/                 # Unit tests with xUnit
│   └── IntegrationTests/          # Integration tests with WebApplicationFactory
├── docker-compose.yml             # PostgreSQL and RabbitMQ services
├── README.md                      # Complete documentation
└── .gitignore                     # Git ignore file
```

## 🚀 Next Steps

### 1. Start Docker Services

```bash
# From project root
docker-compose up -d
```

This starts:
- **PostgreSQL** on port 5432
- **RabbitMQ** on port 5672 (Management UI: http://localhost:15672)

### 2. Create Database Migrations

```bash
cd src/Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Api
dotnet ef database update --startup-project ../Api
```

### 3. Run the Application

```bash
cd src/Api
dotnet run
```

The API will be available at:
- **Swagger UI**: https://localhost:7001/swagger
- **HTTPS**: https://localhost:7001
- **HTTP**: http://localhost:5001

### 4. Run Tests

```bash
# From solution root
dotnet test

# Or individually
dotnet test tests/UnitTests
dotnet test tests/IntegrationTests
```

## 🎯 What's Been Implemented

### ✅ Core Features

**Motorcycles**
- ✅ Create, Read, Update (license plate only), Delete
- ✅ Unique license plate validation
- ✅ RabbitMQ event publishing on registration
- ✅ 2024 year filter for event logging
- ✅ Rental history check before deletion

**Couriers**
- ✅ Registration with validation
- ✅ Unique CNPJ and driver license number
- ✅ Driver license type validation (A, B, A+B)
- ✅ Image upload (PNG/BMP only)
- ✅ Local file storage

**Rentals**
- ✅ Multiple rental plans (7, 15, 30, 45, 50 days)
- ✅ Daily cost calculation based on plan
- ✅ License type validation (only A or A+B)
- ✅ Early return penalty calculation
- ✅ Late return extra charges
- ✅ Total cost calculation

### ✅ Technical Implementation

**Architecture**
- ✅ Clean Architecture (Domain, Application, Infrastructure, API)
- ✅ CQRS pattern with MediatR
- ✅ Repository pattern
- ✅ Unit of Work pattern

**Data & Messaging**
- ✅ Entity Framework Core with PostgreSQL
- ✅ Database configurations and relationships
- ✅ RabbitMQ publisher and consumer
- ✅ Background service for event processing

**Validation & Mapping**
- ✅ FluentValidation for input validation
- ✅ AutoMapper for DTO mapping
- ✅ Custom exception handling

**API**
- ✅ RESTful controllers
- ✅ Swagger/OpenAPI documentation
- ✅ Global exception handling middleware
- ✅ Serilog structured logging

**Testing**
- ✅ xUnit test framework
- ✅ Unit tests with Moq
- ✅ Integration tests with in-memory database
- ✅ FluentAssertions for readable assertions

## 📋 Configuration

### Database Connection (appsettings.json)
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=MotorcycleRentalDb;Username=postgres;Password=postgres"
}
```

### RabbitMQ Configuration
```json
"RabbitMQ": {
  "Host": "localhost",
  "Port": "5672",
  "Username": "guest",
  "Password": "guest"
}
```

### File Storage
```json
"FileStorage": {
  "Path": "uploads"
}
```

## 🔧 Development Tools

### Required
- .NET 8 SDK
- Docker Desktop (for PostgreSQL and RabbitMQ)
- Visual Studio 2022 / VS Code / Rider

### Optional
- Postman or similar for API testing
- pgAdmin for PostgreSQL management
- RabbitMQ Management UI (included in Docker)

## 📊 API Endpoints

### Motorcycles
```
POST   /api/motorcycles                         - Create motorcycle
GET    /api/motorcycles                         - List all (with optional filter)
GET    /api/motorcycles/{id}                    - Get by ID
PUT    /api/motorcycles/{id}/license-plate      - Update license plate
DELETE /api/motorcycles/{id}                    - Delete motorcycle
```

### Couriers
```
POST   /api/couriers                            - Create courier
POST   /api/couriers/{id}/driver-license-image  - Upload license image
```

### Rentals
```
POST   /api/rentals                             - Create rental
GET    /api/rentals/{id}                        - Get rental by ID
PUT    /api/rentals/{id}/return                 - Return motorcycle
```

## 🧪 Test Coverage

### Unit Tests
- Domain entity business logic (rental cost calculations)
- Command handlers (create motorcycle, courier)
- Validation rules
- Edge cases

### Integration Tests
- Full API workflow
- Database operations
- HTTP endpoints
- Data persistence

## 📝 Logging

Logs are written to:
- Console (real-time)
- Files: `logs/log-{Date}.txt` (daily rotation)

## ⚠️ Important Notes

1. **Database**: Run migrations before first use
2. **RabbitMQ**: Ensure Docker services are running
3. **File Storage**: `uploads/` directory created automatically
4. **Tests**: Use in-memory database (no external dependencies)
5. **Swagger**: Available only in Development environment

## 🎓 Code Quality

- ✅ English code and comments
- ✅ RESTful conventions
- ✅ Proper exception handling
- ✅ Structured logging
- ✅ Dependency injection
- ✅ Clean Architecture principles
- ✅ No company names in code

## 🚨 Troubleshooting

### Docker services not starting
```bash
docker-compose down
docker-compose up -d
```

### Database connection errors
- Check if PostgreSQL is running: `docker ps`
- Verify connection string in appsettings.json
- Ensure migrations are applied

### RabbitMQ connection errors
- Check if RabbitMQ is running: `docker ps`
- Verify RabbitMQ configuration
- Check Management UI: http://localhost:15672

### Build warnings
- AutoMapper version warnings are expected and won't affect functionality
- Entity Framework version conflicts resolved by using version 9.0.10

## 📚 Additional Resources

- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [RabbitMQ](https://www.rabbitmq.com/documentation.html)

## ✅ Project Status: COMPLETE

All requirements have been implemented and tested. The application is ready for development and testing!
