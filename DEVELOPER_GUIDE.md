# Developer Guide - BallastLane Test

This guide provides detailed information for developers working on the BallastLane Test project.

## Table of Contents
1. [Project Setup](#project-setup)
2. [Development Workflow](#development-workflow)
3. [Architecture Deep Dive](#architecture-deep-dive)
4. [Database Management](#database-management)
5. [Testing](#testing)
6. [Common Tasks](#common-tasks)
7. [Troubleshooting](#troubleshooting)

---

## Project Setup

### Initial Setup (First Time)

1. **Clone Repository**
   ```bash
   git clone https://github.com/simakmatveistash-collab/BallastLaneTest
   cd BallastLaneTest/BallastLaneTest
   ```

2. **Verify .NET Installation**
   ```bash
   dotnet --version  # Should be 10.0 or higher
   ```

3. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

4. **Build Solution**
   ```bash
   dotnet build
   ```

5. **Run Tests**
   ```bash
   dotnet test
   ```

6. **Start Application**
   ```bash
   cd BallastLaneTest.Server
   dotnet run
   ```

### IDE Setup

#### Visual Studio Community 2026
1. Open `BallastLaneTest.slnx`
2. Solution should auto-restore packages
3. Set `BallastLaneTest.Server` as startup project
4. Press F5 to debug

#### Visual Studio Code
1. Install C# extension
2. Install .NET Install Tool extension
3. Open workspace in VSCode
4. Create `.vscode/launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
	{
	  "name": ".NET Core Launch (web)",
	  "type": "coreclr",
	  "request": "launch",
	  "preLaunchTask": "build",
	  "program": "${workspaceFolder}/BallastLaneTest.Server/bin/Debug/net10.0/BallastLaneTest.Server.dll",
	  "args": [],
	  "cwd": "${workspaceFolder}/BallastLaneTest.Server",
	  "stopAtEntry": false,
	  "serverReadyAction": {
		"pattern": "\\bNow listening on:\\s+(https?://\\S+)",
		"uriFormat": "{0}",
		"action": "openExternally"
	  }
	}
  ]
}
```

---

## Development Workflow

### Daily Workflow

```bash
# 1. Pull latest changes
git pull origin master

# 2. Restore and rebuild
dotnet restore
dotnet build

# 3. Run tests to ensure nothing broke
dotnet test

# 4. Start development server
cd BallastLaneTest.Server
dotnet run

# 5. Access application
# Browser: https://localhost:5001
# Swagger: https://localhost:5001/openapi/v1.json
```

### Adding a New Feature

1. **Create feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Write tests first (TDD)**
   - Add test in appropriate Test project
   - Ensure test fails initially

3. **Implement feature**
   - Follow Clean Architecture layers
   - Add domain entity if needed
   - Create application service
   - Add infrastructure implementation
   - Create API endpoints

4. **Verify tests pass**
   ```bash
   dotnet test
   ```

5. **Commit changes**
   ```bash
   git add .
   git commit -m "feat: description of your feature"
   ```

6. **Push and create PR**
   ```bash
   git push origin feature/your-feature-name
   ```

### Code Style

We follow Microsoft's C# Coding Conventions:
- Use camelCase for private fields and local variables
- Use PascalCase for public members and constants
- Use meaningful names (no single letter variables except in loops)
- 4-space indentation
- Opening braces on same line (Allman style adjusted)

Example:
```csharp
public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;

	public UserService(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<UserDto> GetUserAsync(int id, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(id, cancellationToken);
		if (user == null)
			throw new InvalidOperationException($"User with ID {id} not found");

		return new UserDto
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email
		};
	}
}
```

---

## Architecture Deep Dive

### Project Layers

#### 1. Domain Layer (`BallastLaneTest.Domain`)
**Responsibility**: Pure business logic and entities

**Files**:
- `Entities/`: Domain models (User, Record)
- `Repositories/`: Repository interfaces (define data access contracts)

**Key Points**:
- No dependencies on other projects
- No external libraries (except those explicitly needed for domain)
- Contains aggregate roots and value objects
- Repository interfaces are defined here

**Example**:
```csharp
// Domain/Entities/User.cs
public class User
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }

	public virtual ICollection<Record> Records { get; set; } = [];
}

// Domain/Repositories/IUserRepository.cs
public interface IUserRepository
{
	Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	// ... other methods
}
```

#### 2. Application Layer (`BallastLaneTest.Application`)
**Responsibility**: Use cases and business rules

**Files**:
- `Dtos/`: Data Transfer Objects (contracts for API)
- `Services/`: Business logic implementation

**Key Points**:
- Depends on Domain layer only
- Contains application services (orchestrate domain logic)
- DTOs map domain models to API contracts
- Service interfaces defined here, implementations in Infrastructure

**Example**:
```csharp
// Application/Services/IAuthService.cs
public interface IAuthService
{
	Task<UserDto> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
	Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

// Application/Services/Implementations/AuthService.cs
public class AuthService : IAuthService
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;

	public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
	{
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
	}

	public async Task<UserDto> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
	{
		// Business logic implementation
		if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
			throw new InvalidOperationException("Email already registered");

		var user = new User
		{
			Name = request.Name,
			Email = request.Email,
			PasswordHash = _passwordHasher.HashPassword(request.Password),
			CreatedDate = DateTime.UtcNow
		};

		await _userRepository.AddAsync(user, cancellationToken);
		return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email };
	}
}
```

#### 3. Infrastructure Layer (`BallastLaneTest.Infrastructure`)
**Responsibility**: External integrations and data access

**Files**:
- `Data/`: Entity Framework DbContext and configurations
- `Repositories/`: Repository implementations
- `Migrations/`: EF Core migrations
- `Security/`: Password hashing

**Key Points**:
- Depends on Application and Domain layers
- Implements interfaces from Application layer
- Handles all external concerns (database, APIs, file systems)
- Migration files versioned here

**Example**:
```csharp
// Infrastructure/Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<Record> Records { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// User Configuration
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).HasMaxLength(256).IsRequired();
			entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
			entity.HasIndex(e => e.Email).IsUnique();
			entity.Property(e => e.PasswordHash).IsRequired();
			entity.Property(e => e.CreatedDate).IsRequired();

			entity.HasMany(u => u.Records)
				.WithOne(r => r.User)
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// Record Configuration
		modelBuilder.Entity<Record>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
			entity.Property(e => e.Content).IsRequired();
			entity.Property(e => e.UserId).IsRequired();
			entity.Property(e => e.CreatedDate).IsRequired();
			entity.Property(e => e.UpdatedDate).IsRequired();

			entity.HasOne(r => r.User)
				.WithMany(u => u.Records)
				.HasForeignKey(r => r.UserId);
		});
	}
}

// Infrastructure/Repositories/UserRepository.cs
public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _context;

	public UserRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
	}

	public async Task AddAsync(User user, CancellationToken cancellationToken = default)
	{
		_context.Users.Add(user);
		await _context.SaveChangesAsync(cancellationToken);
	}

	// ... other methods
}
```

#### 4. Server/API Layer (`BallastLaneTest.Server`)
**Responsibility**: HTTP request handling and API composition

**Files**:
- `Program.cs`: Application startup configuration
- `Extensions.cs`: Dependency injection and service registration
- `Endpoints/`: Minimal API route handlers
- `Middleware/`: Custom HTTP middleware
- `Startup/`: Database seeding and initialization

**Key Points**:
- Depends on all layers
- Minimal APIs for lightweight HTTP handling
- Maps HTTP requests to business logic
- Configures dependency injection
- Middleware pipeline for cross-cutting concerns

**Example**:
```csharp
// Server/Endpoints/UserEndpoints.cs
public static class UserEndpoints
{
	public static void MapUserEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/api/users")
			.WithTags("Users")
			.WithOpenApi();

		group.MapPost("/register", Register)
			.WithName("RegisterUser")
			.WithSummary("Register a new user");
	}

	private static async Task<IResult> Register(
		RegisterUserRequest request,
		IAuthService authService,
		CancellationToken cancellationToken)
	{
		try
		{
			var user = await authService.RegisterUserAsync(request, cancellationToken);
			return Results.Created($"/api/users/{user.Id}", user);
		}
		catch (InvalidOperationException ex)
		{
			return Results.BadRequest(new { message = ex.Message });
		}
	}
}
```

#### 5. Tests Layer (`BallastLaneTest.Tests`)
**Responsibility**: Quality assurance and regression prevention

**Files**:
- `Repositories/`: Tests for data access layer
- `Services/`: Tests for business logic
- `Fixtures/`: Test utilities and fixtures

**Key Points**:
- NUnit testing framework
- Moq for mocking dependencies
- In-memory database for repository tests
- Isolated unit tests with no external dependencies

**Example**:
```csharp
// Tests/Services/AuthServiceTests.cs
public class AuthServiceTests
{
	private Mock<IUserRepository> _mockUserRepository;
	private Mock<IPasswordHasher> _mockPasswordHasher;
	private AuthService _authService;

	[SetUp]
	public void Setup()
	{
		_mockUserRepository = new Mock<IUserRepository>();
		_mockPasswordHasher = new Mock<IPasswordHasher>();
		_authService = new AuthService(_mockUserRepository.Object, _mockPasswordHasher.Object);
	}

	[Test]
	public async Task RegisterUserAsync_Should_Create_New_User()
	{
		// Arrange
		var request = new RegisterUserRequest 
		{ 
			Name = "John", 
			Email = "john@example.com", 
			Password = "Password123!" 
		};

		_mockUserRepository.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>()))
			.Returns("hashed_password");

		// Act
		var result = await _authService.RegisterUserAsync(request);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Email, Is.EqualTo("john@example.com"));
	}
}
```

---

## Database Management

### Creating Migrations

When domain models change, create a new migration:

```bash
cd BallastLaneTest.Server

# Add new migration
dotnet ef migrations add MigrationName --project ../BallastLaneTest.Infrastructure

# View pending migrations
dotnet ef migrations list --project ../BallastLaneTest.Infrastructure

# Generate SQL script
dotnet ef migrations script --project ../BallastLaneTest.Infrastructure --output migration.sql
```

### Applying Migrations

Migrations apply automatically on application startup:
```csharp
// In Extensions.cs ApplyMigrations method
public static WebApplication ApplyMigrations(this WebApplication app)
{
	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		dbContext.Database.Migrate();  // Applies pending migrations
	}
	return app;
}
```

### Reverting Migrations

```bash
cd BallastLaneTest.Server

# Remove last migration (if not applied)
dotnet ef migrations remove --project ../BallastLaneTest.Infrastructure

# Revert to specific database state
dotnet ef database update PreviousMigrationName --project ../BallastLaneTest.Infrastructure
```

### Database Seeding

Test data seeds in development mode:
```csharp
// Server/Startup/DatabaseSeeder.cs
public static class DatabaseSeeder
{
	public static async Task SeedDataAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		if (dbContext.Users.Any())
			return; // Already seeded

		// Add seed users and records
		var user = new User { Name = "Test User", Email = "test@example.com", ... };
		dbContext.Users.Add(user);
		await dbContext.SaveChangesAsync();
	}
}
```

---

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific project
dotnet test BallastLaneTest.Tests/BallastLaneTest.Tests.csproj

# Run specific test class
dotnet test --filter "ClassName=UserRepositoryTests"

# Run with verbosity
dotnet test --verbosity detailed

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Writing Tests

#### Repository Tests (Integration)
Test database operations with real EF Core:
```csharp
[TestFixture]
public class UserRepositoryTests
{
	private ApplicationDbContext _dbContext;
	private UserRepository _repository;

	[SetUp]
	public void Setup()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		_dbContext = new ApplicationDbContext(options);
		_repository = new UserRepository(_dbContext);
	}

	[TearDown]
	public void Teardown()
	{
		_dbContext?.Dispose();
	}

	[Test]
	public async Task AddAsync_Should_Create_New_User()
	{
		// Arrange
		var user = new User { Name = "John", Email = "john@example.com", PasswordHash = "hash" };

		// Act
		await _repository.AddAsync(user);

		// Assert
		var found = await _repository.GetByIdAsync(user.Id);
		Assert.That(found, Is.Not.Null);
		Assert.That(found.Email, Is.EqualTo("john@example.com"));
	}
}
```

#### Service Tests (Unit)
Test business logic with mocks:
```csharp
[TestFixture]
public class AuthServiceTests
{
	private Mock<IUserRepository> _mockUserRepository;
	private AuthService _service;

	[SetUp]
	public void Setup()
	{
		_mockUserRepository = new Mock<IUserRepository>();
		_service = new AuthService(_mockUserRepository.Object, new PasswordHasher());
	}

	[Test]
	public async Task LoginAsync_Should_Return_Token_When_Valid()
	{
		// Arrange
		var user = new User 
		{ 
			Id = 1, 
			Email = "john@example.com", 
			PasswordHash = new PasswordHasher().HashPassword("password") 
		};
		_mockUserRepository.Setup(x => x.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		// Act
		var result = await _service.LoginAsync(new LoginRequest { Email = "john@example.com", Password = "password" });

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.UserId, Is.EqualTo(1));
	}
}
```

### Test Best Practices

1. **Arrange-Act-Assert Pattern**: Clear test structure
2. **One Assertion per Test**: Tests should validate single behavior
3. **Meaningful Names**: Test name describes what is being tested
4. **Isolated Tests**: No dependencies between tests
5. **Mock External Dependencies**: Use Moq for infrastructure
6. **Use Fixtures**: DatabaseFixture for repeated setup

---

## Common Tasks

### Add a New Entity

1. Create entity in `Domain/Entities/`:
```csharp
public class Task
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public bool IsCompleted { get; set; }
	public int UserId { get; set; }
	public DateTime CreatedDate { get; set; }

	public virtual User? User { get; set; }
}
```

2. Create repository interface in `Domain/Repositories/`:
```csharp
public interface ITaskRepository
{
	Task<Domain.Entities.Task?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<IEnumerable<Domain.Entities.Task>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
	Task AddAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default);
	Task UpdateAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default);
	Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
```

3. Add DbSet to context in `Infrastructure/Data/ApplicationDbContext.cs`:
```csharp
public DbSet<Task> Tasks { get; set; }
```

4. Configure entity in `OnModelCreating`:
```csharp
modelBuilder.Entity<Task>(entity =>
{
	entity.HasKey(e => e.Id);
	entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
	entity.HasOne(t => t.User)
		.WithMany()
		.HasForeignKey(t => t.UserId)
		.OnDelete(DeleteBehavior.Cascade);
});
```

5. Implement repository in `Infrastructure/Repositories/`:
```csharp
public class TaskRepository : ITaskRepository
{
	private readonly ApplicationDbContext _context;

	public TaskRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Task?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
	}

	public async Task AddAsync(Task task, CancellationToken cancellationToken = default)
	{
		_context.Tasks.Add(task);
		await _context.SaveChangesAsync(cancellationToken);
	}

	// ... other methods
}
```

6. Create service if needed in `Application/Services/`:
```csharp
public interface ITaskService
{
	Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, int userId, CancellationToken cancellationToken = default);
	Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId, CancellationToken cancellationToken = default);
}

public class TaskService : ITaskService
{
	private readonly ITaskRepository _taskRepository;

	public TaskService(ITaskRepository taskRepository)
	{
		_taskRepository = taskRepository;
	}

	public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, int userId, CancellationToken cancellationToken = default)
	{
		var task = new Task
		{
			Title = request.Title,
			Description = request.Description,
			UserId = userId,
			CreatedDate = DateTime.UtcNow,
			IsCompleted = false
		};

		await _taskRepository.AddAsync(task, cancellationToken);
		return new TaskDto { Id = task.Id, Title = task.Title };
	}

	public async Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId, CancellationToken cancellationToken = default)
	{
		var tasks = await _taskRepository.GetByUserIdAsync(userId, cancellationToken);
		return tasks.Select(t => new TaskDto { Id = t.Id, Title = t.Title, IsCompleted = t.IsCompleted });
	}
}
```

7. Register in DI in `Server/Extensions.cs`:
```csharp
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
```

8. Add endpoints in `Server/Endpoints/TaskEndpoints.cs`:
```csharp
public static class TaskEndpoints
{
	public static void MapTaskEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/api/tasks")
			.WithTags("Tasks")
			.WithOpenApi();

		group.MapGet("", GetTasks).WithName("GetUserTasks");
		group.MapPost("", CreateTask).WithName("CreateTask");
	}

	private static async Task<IResult> GetTasks(ITaskService service, HttpContext context, CancellationToken cancellationToken)
	{
		var userId = GetUserIdFromContext(context);
		if (userId <= 0) return Results.Unauthorized();

		var tasks = await service.GetUserTasksAsync(userId, cancellationToken);
		return Results.Ok(tasks);
	}

	private static async Task<IResult> CreateTask(CreateTaskRequest request, ITaskService service, HttpContext context, CancellationToken cancellationToken)
	{
		var userId = GetUserIdFromContext(context);
		if (userId <= 0) return Results.Unauthorized();

		var task = await service.CreateTaskAsync(request, userId, cancellationToken);
		return Results.Created($"/api/tasks/{task.Id}", task);
	}

	private static int GetUserIdFromContext(HttpContext context)
	{
		if (context.Items.TryGetValue("UserId", out var userId) && userId is int id)
			return id;
		return 0;
	}
}
```

9. Create migration:
```bash
cd BallastLaneTest.Server
dotnet ef migrations add AddTaskEntity --project ../BallastLaneTest.Infrastructure
```

10. Write tests in `Tests/Repositories/TaskRepositoryTests.cs` and `Tests/Services/TaskServiceTests.cs`

---

## Troubleshooting

### Build Fails with Package Errors

```bash
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore --no-cache
dotnet build
```

### Database Connection Issues

```bash
# Check SQL Server LocalDB is running
sqllocaldb info

# Start LocalDB instance
sqllocaldb start mssqllocaldb

# Create instance if needed
sqllocaldb create mssqllocaldb
```

### Migrations Won't Apply

```bash
# Check pending migrations
dotnet ef migrations list --project BallastLaneTest.Infrastructure --startup-project BallastLaneTest.Server

# Reset database (DESTRUCTIVE!)
dotnet ef database drop --force --project BallastLaneTest.Infrastructure --startup-project BallastLaneTest.Server

# Reapply migrations
dotnet ef database update --project BallastLaneTest.Infrastructure --startup-project BallastLaneTest.Server
```

### Tests Won't Run

```bash
# Rebuild test project
dotnet clean BallastLaneTest.Tests
dotnet build BallastLaneTest.Tests

# Run with verbose output
dotnet test BallastLaneTest.Tests --verbosity detailed
```

### Port 5001 Already in Use

```bash
# Windows: Find and kill process
netstat -ano | findstr :5001
taskkill /PID {PID} /F

# Linux/Mac:
lsof -ti:5001 | xargs kill -9

# Or change port in launchSettings.json
```

### IntelliSense Not Working

1. Reload window in VS Code
2. Restart Visual Studio
3. Check `.csproj` files have correct framework:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

---

## Resources

- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [NUnit Documentation](https://docs.nunit.org/)
- [Moq Documentation](https://github.com/moq/moq4)

---

For questions or issues, please open an issue on GitHub or contact the development team.
