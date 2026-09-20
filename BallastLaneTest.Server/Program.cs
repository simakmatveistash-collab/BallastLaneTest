using BallastLaneTest.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add application services with dependency injection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=BallastLaneTest;Trusted_Connection=true;";
builder.AddApplicationServices(connectionString);

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS for frontend communication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply database migrations
app.ApplyMigrations();

// Seed database with test data in development
if (app.Environment.IsDevelopment())
{
    await app.SeedDatabaseAsync();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Use CORS
app.UseCors("AllowFrontend");

// Apply custom middleware (includes authentication)
app.UseCustomMiddleware();

// Add routing and authorization
app.UseRouting();
app.UseAuthorization();

// Map API endpoints
app.MapAuthEndpoints();
app.MapRecordEndpoints();

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
