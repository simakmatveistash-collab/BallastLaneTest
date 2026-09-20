# BallastLane Test - Clean Architecture Application

A modern .NET 10 web application implementing Clean Architecture principles with ASP.NET Core Minimal APIs, Entity Framework Core, and comprehensive testing.

## Architecture Overview

The application is structured following Clean Architecture principles with clear separation of concerns:

```
BallastLaneTest/
├── BallastLaneTest.Domain/              # Core business logic & entities
│   ├── Entities/                        # Domain models (User, Record)
│   └── Repositories/                    # Repository interfaces
├── BallastLaneTest.Application/         # Use cases & business rules
│   ├── Dtos/                            # Data transfer objects
│   └── Services/                        # Business logic services
├── BallastLaneTest.Infrastructure/      # Data access & external services
│   ├── Data/                            # DbContext & EF Core configuration
│   ├── Repositories/                    # Repository implementations
│   ├── Migrations/                      # Database migrations
│   └── Security/                        # Password hashing & security
├── BallastLaneTest.Server/              # API & Composition Root
│   ├── Endpoints/                       # Minimal API route handlers
│   ├── Middleware/                      # Custom HTTP middleware
│   └── Startup/                         # Startup configuration & seeding
└── BallastLaneTest.Tests/               # Unit & Integration tests
	├── Repositories/                    # Repository tests
	└── Services/                        # Service tests
```

## Technology Stack

- **.NET**: 10.0
- **Framework**: ASP.NET Core with Minimal APIs
- **Database**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core
- **Authentication**: Token-based (simple Base64 encoding)
- **Testing**: NUnit with Moq
- **API Documentation**: OpenAPI/Swagger
- **Hosting**: Aspire (for development orchestration)

## Database Schema

### Users Table
- `Id` (int, PK): Unique user identifier
- `Name` (string, max 256): User's display name
- `Email` (string, max 256, unique index): User's email address
- `PasswordHash` (string): PBKDF2-hashed password
- `CreatedDate` (datetime): User creation timestamp

### Records Table
- `Id` (int, PK): Unique record identifier
- `Title` (string, max 500): Record title
- `Content` (string): Record content/description
- `UserId` (int, FK): Reference to owning user
- `CreatedDate` (datetime): Record creation timestamp
- `UpdatedDate` (datetime): Record last update timestamp

## Features

### Authentication (`/api/auth`)
- **POST /api/auth/register**: Register a new user
  ```json
  {
	"name": "John Doe",
	"email": "john@example.com",
	"password": "SecurePassword123!"
  }
  ```
  Returns: User object with ID and authentication token

- **POST /api/auth/login**: Login with credentials
  ```json
  {
	"email": "john@example.com",
	"password": "SecurePassword123!"
  }
  ```
  Returns: Login response with authentication token

### Records Management (`/api/records`)
- **GET /api/records**: Get all records for authenticated user
- **GET /api/records/{id}**: Get a specific record by ID (with ownership verification)
- **POST /api/records**: Create a new record
  ```json
  {
	"title": "My Task",
	"content": "Task description here"
  }
  ```
- **PUT /api/records/{id}**: Update an existing record (owner only)
- **DELETE /api/records/{id}**: Delete a record (owner only)

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server or SQL Server Express (LocalDB)
- Visual Studio 2026+ or VS Code

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/simakmatveistash-collab/BallastLaneTest
   cd BallastLaneTest/BallastLaneTest
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run migrations (automatic on startup)**
   The database migrations run automatically when the application starts in development mode.

5. **Seed test data**
   Test data is automatically seeded in development mode:
   - User: john@example.com / Password123!
   - User: jane@example.com / SecurePass456!
   - Sample records for testing

6. **Run the application**
   ```bash
   cd BallastLaneTest.Server
   dotnet run
   ```

   Or using Aspire:
   ```bash
   cd BallastLaneTest.AppHost
   dotnet run
   ```

7. **Access the API**
   - API Base URL: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/openapi/v1.json`

### Run Tests

Execute all unit tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test BallastLaneTest.Tests/BallastLaneTest.Tests.csproj
```

Run with coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

## API Examples

### Register a User
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
	"name": "Alice Johnson",
	"email": "alice@example.com",
	"password": "MyPassword123!"
  }'
```

### Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "alice@example.com",
	"password": "MyPassword123!"
  }'
```

### Create a Record (requires userId in header or query)
```bash
curl -X POST https://localhost:5001/api/records?userId=1 \
  -H "Content-Type: application/json" \
  -d '{
	"title": "Complete Project",
	"content": "Finish the Clean Architecture implementation"
  }'
```

### Get User's Records
```bash
curl -X GET https://localhost:5001/api/records?userId=1
```

## Security Features

- **Password Hashing**: PBKDF2 with salt (not MD5 or plain text)
- **Ownership Verification**: Record operations validate user ownership
- **CORS**: Configured for frontend integration
- **Authentication Middleware**: Extracts and validates user identity from tokens
- **Exception Handling**: Global middleware for consistent error responses

## Testing Strategy

The project includes comprehensive test coverage:

### Repository Tests (EF Core)
- CRUD operations validation
- Database constraints verification
- Query correctness

### Service Tests (Business Logic)
- Registration & login workflows
- Record CRUD with ownership enforcement
- Error handling & validation

### Fixtures & Helpers
- In-memory database for isolated tests
- Mock repository implementations
- Reusable test data builders

## Configuration

### Connection String (appsettings.json)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BallastLaneTestDb;Trusted_Connection=true;"
  }
}
```

### Environment-Based Configuration
- **Development**: Automatic migrations, seed data, Swagger enabled
- **Production**: Manual migration application, no seed data, restrictions enabled

## Project Structure Best Practices

1. **Dependency Injection**: All services registered in `Extensions.cs`
2. **Repository Pattern**: Data access abstracted through interfaces
3. **Service Layer**: Business logic separated from infrastructure
4. **DTOs**: Domain entities mapped to contract objects
5. **Middleware**: Cross-cutting concerns (auth, error handling) isolated
6. **Testing**: Comprehensive with mocks and fixtures

## Git Strategy

The repository uses a master branch for stable code:
- Initialize with planned features
- Test thoroughly before commits
- Document changes in commit messages

## Future Enhancements

- [ ] JWT token implementation
- [ ] Role-based access control (RBAC)
- [ ] API versioning
- [ ] Rate limiting
- [ ] Caching strategies
- [ ] Logging & monitoring
- [ ] Docker containerization
- [ ] CI/CD pipeline setup

## Troubleshooting

### Database Connection Issues
```bash
# Check LocalDB instance list
sqllocaldb info

# Create/start LocalDB instance if needed
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### Migration Errors
```bash
# View migration status
dotnet ef migrations list --project BallastLaneTest.Infrastructure --startup-project BallastLaneTest.Server

# Reset database (destructive!)
dotnet ef database drop --project BallastLaneTest.Infrastructure --startup-project BallastLaneTest.Server
```

### Port Already in Use
Modify port in `launchSettings.json`:
```json
"profiles": {
  "https": {
	"commandName": "Project",
	"launchBrowser": true,
	"launchUrl": "openapi/v1.json",
	"applicationUrl": "https://localhost:YOUR_PORT",
	...
  }
}
```

## License

This project is provided as-is for educational and testing purposes.

## Contributing

Contributions are welcome! Please ensure:
1. All tests pass
2. Code follows C# conventions
3. New features include tests
4. Documentation is updated
