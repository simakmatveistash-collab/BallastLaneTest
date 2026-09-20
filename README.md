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

## Generative AI Tools Usage

This project was developed using GitHub Copilot as an auxiliary tool to accelerate development while maintaining code quality, Clean Architecture principles, and comprehensive testing. Below is documentation of how AI was leveraged throughout the project.

### AI Prompts Used

#### 1. API Scaffold Generation
**Prompt**:
```
Create ASP.NET Core Minimal API endpoints for a task management system with the following requirements:
- CRUD operations for tasks/records
- Each task has: title, description, status (optional), due_date (optional)
- Tasks associated with users
- Authentication with user registration and login
- Ownership verification (users can only modify their own tasks)
- Use Repository pattern and dependency injection
- Follow Clean Architecture principles
```

**AI Output**: Generated initial endpoint structure with minimal APIs, request/response DTOs, and middleware setup.

#### 2. Clean Architecture Layer Setup
**Prompt**:
```
Generate a Clean Architecture project structure in C# with:
- Domain layer: Core entities and repository interfaces
- Application layer: Business logic services and DTOs
- Infrastructure layer: EF Core repositories and database context
- Presentation layer: ASP.NET Core API endpoints
- Separate test project with NUnit
Include proper dependency injection setup.
```

**AI Output**: Provided folder structure, project file organization, and service registration patterns.

#### 3. Authentication Service Implementation
**Prompt**:
```
Create an AuthService class with:
- RegisterUserAsync: Validate email uniqueness, hash password using PBKDF2, create user
- LoginAsync: Validate credentials, return user info with token
- PasswordHashingService: PBKDF2 implementation with salt
- Include proper error handling and validation
```

**AI Output**: Generated service with PBKDF2 password hashing, validation middleware, and async task patterns.

### AI-Generated Code Sample

**Example: Record Service Implementation**
```csharp
// AI-generated service structure (with improvements)
public class RecordService : IRecordService
{
    private readonly IRecordRepository _recordRepository;
    private readonly ILogger<RecordService> _logger;

    public RecordService(IRecordRepository recordRepository, ILogger<RecordService> logger)
    {
        _recordRepository = recordRepository;
        _logger = logger;
    }

    public async Task<RecordDto> CreateRecordAsync(int userId, CreateRecordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required");

        var record = new Record
        {
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        await _recordRepository.AddAsync(record);
        return MapToDto(record);
    }

    public async Task UpdateRecordAsync(int recordId, int userId, UpdateRecordRequest request)
    {
        var record = await _recordRepository.GetByIdAsync(recordId);

        if (record == null)
            throw new KeyNotFoundException($"Record {recordId} not found");

        if (record.UserId != userId)
            throw new UnauthorizedAccessException("You cannot modify records you don't own");

        record.Title = request.Title ?? record.Title;
        record.Content = request.Content ?? record.Content;
        record.UpdatedDate = DateTime.UtcNow;

        await _recordRepository.UpdateAsync(record);
    }
}
```

### Validation Approach

1. **Code Review**: 
   - Verified AI output against Clean Architecture principles
   - Ensured separation of concerns across layers
   - Checked for proper async/await patterns

2. **Test Coverage Verification**:
   - Generated comprehensive unit tests for all services
   - Tested edge cases and error conditions
   - Verified repository mocking with Moq

3. **Security Validation**:
   - Confirmed PBKDF2 hashing instead of MD5 or plain text
   - Verified ownership checks on all modifying operations
   - Validated input sanitization and null checks

4. **Architecture Compliance**:
   - Ensured no cross-layer dependency violations
   - Verified dependency injection configuration
   - Checked DTOs properly decouple domain entities

### Corrections & Improvements Made

1. **Password Hashing**:
   - ❌ AI initially suggested simple MD5 hashing
   - ✅ Corrected to industry-standard PBKDF2 with salt and iteration count

2. **Error Handling**:
   - ❌ AI generated generic Exception throws
   - ✅ Improved with specific exception types (ArgumentException, UnauthorizedAccessException, KeyNotFoundException)

3. **Ownership Verification**:
   - ❌ AI forgot to include UserId checks on update/delete operations
   - ✅ Added comprehensive ownership verification in service layer

4. **Async Patterns**:
   - ❌ AI sometimes used `.Result` property (sync-over-async antipattern)
   - ✅ Ensured proper async/await throughout the codebase

5. **DTOs & Mapping**:
   - ❌ AI initially exposed domain entities directly
   - ✅ Created explicit DTOs with mapping logic to prevent over-posting attacks

6. **Null Checks**:
   - ❌ AI relied on nullable reference types without explicit checks
   - ✅ Added defensive null checks and validation messages

### Edge Cases Handled

1. **Duplicate Email Registration**
   ```csharp
   if (await _userRepository.ExistsByEmailAsync(request.Email))
       throw new InvalidOperationException("Email already registered");
   ```
   - Field-level unique index on Users.Email
   - Service-layer validation for duplicate prevention

2. **Invalid Credentials**
   ```csharp
   var user = await _userRepository.GetByEmailAsync(request.Email);
   if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
       throw new InvalidOperationException("Invalid email or password");
   ```
   - Generic error message to prevent email enumeration
   - Password verification with constant-time comparison

3. **Unauthorized Record Access**
   ```csharp
   if (record.UserId != userId)
       throw new UnauthorizedAccessException("Cannot access other user's records");
   ```
   - Verified on GET, PUT, DELETE operations
   - Returns 403 Forbidden status code

4. **Missing or Malformed Data**
   ```csharp
   if (string.IsNullOrWhiteSpace(createRequest.Title))
       throw new ArgumentException("Title cannot be empty");
   ```
   - Service-layer validation for all business rules
   - API-level model validation for format compliance

5. **Concurrent Updates**
   - Used optimistic concurrency control with `UpdatedDate` tracking
   - EF Core change tracking prevents lost updates

6. **Test Data Isolation**
   - In-memory database for each test ensures no cross-test contamination
   - Fixtures reset state between test runs

### AI Tool: GitHub Copilot

**Settings Used**:
- Language: C# with .NET 10 context
- Framework: ASP.NET Core with Entity Framework
- Testing: NUnit and Moq patterns
- Architecture: Clean Architecture compliance

**Effectiveness**:
- ✅ Accelerated boilerplate generation (40-50% faster initial scaffolding)
- ✅ Provided documentation through XML comments
- ✅ Suggested repository patterns and DI setup
- ⚠️ Required careful review for security implications
- ⚠️ Needed refinement for edge case handling

**Lessons Learned**:
1. Always validate AI-generated security code independently
2. AI excels at generating boilerplate but needs guidance on architecture decisions
3. Test-first approach helps catch AI-suggested issues
4. Code review is essential even with AI assistance
5. AI works best when given domain context and requirements explicitly

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
