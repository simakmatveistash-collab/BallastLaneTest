# Full Stack Application Setup & Testing Guide

## Overview

This guide covers how to run and test the complete BallastLaneTest application with its new React frontend and .NET backend.

## Prerequisites

- .NET 10 SDK
- Node.js >= 20.19.0
- npm >= 10.0.0
- Visual Studio Community 2026 (or command line tools)
- SQL Server or LocalDB

## Project Structure

```
BallastLaneTest/
├── BallastLaneTest.Server/          # ASP.NET Core API
├── BallastLaneTest.Application/     # Business logic & DTOs
├── BallastLaneTest.Domain/          # Domain entities
├── BallastLaneTest.Infrastructure/  # Database & persistence
├── BallastLaneTest.AppHost/         # .NET Aspire orchestration
├── BallastLaneTest.Tests/           # Backend tests
└── frontend/                        # React frontend
```

## Backend Setup

### 1. Database Configuration

The backend uses Entity Framework Core with SQL Server/LocalDB.

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BallastLaneTest;Trusted_Connection=true;"
  }
}
```

### 2. Run Database Migrations

Navigate to the backend directory:
```bash
cd BallastLaneTest.Server
dotnet ef database update
```

Or apply migrations from Package Manager Console:
```
Update-Database
```

### 3. Start the Backend

**Option A: Using .NET Aspire (Recommended)**
```bash
cd BallastLaneTest.AppHost
dotnet run
```

This will start the entire application stack with service orchestration.

**Option B: Direct Execution**
```bash
cd BallastLaneTest.Server
dotnet run --launch-profile Development
```

The backend API will be available at: `http://localhost:5000`

## Frontend Setup

### 1. Install Dependencies

```bash
cd frontend
npm install
```

### 2. Configure Environment

Create `.env.development` in the frontend directory:
```env
VITE_API_URL=http://localhost:5000
```

### 3. Start Development Server

```bash
npm run dev
```

The frontend will be available at: `http://localhost:5173`

### 4. Build for Production

```bash
npm run build
npm run preview
```

## Running the Full Application

### Development Environment

**Terminal 1 - Start Backend API:**
```bash
cd BallastLaneTest.Server
dotnet run
```

**Terminal 2 - Start Frontend Dev Server:**
```bash
cd frontend
npm run dev
```

Then open: `http://localhost:5173`

### Using .NET Aspire (All-in-One)

```bash
cd BallastLaneTest.AppHost
dotnet run
```

This orchestrates all services including:
- Backend API
- Database
- Frontend build server

## API Endpoints Reference

### Authentication Endpoints

**Register User**
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "Password123"
}

Response: 201 Created
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com"
}
```

**Login**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123"
}

Response: 200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
	"id": 1,
	"name": "John Doe",
	"email": "john@example.com"
  }
}
```

### Records Endpoints (Requires Authentication)

**Get All Records**
```http
GET /api/records
Authorization: Bearer {token}

Response: 200 OK
[
  {
	"id": 1,
	"title": "My First Record",
	"content": "Record content here...",
	"userId": 1,
	"createdDate": "2024-01-15T10:30:00Z",
	"updatedDate": "2024-01-15T10:30:00Z"
  }
]
```

**Create Record**
```http
POST /api/records
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "New Record",
  "content": "This is the content of my record"
}

Response: 201 Created
```

**Get Record by ID**
```http
GET /api/records/1
Authorization: Bearer {token}

Response: 200 OK
{
  "id": 1,
  "title": "My Record",
  "content": "Content...",
  "userId": 1,
  "createdDate": "2024-01-15T10:30:00Z",
  "updatedDate": "2024-01-15T10:30:00Z"
}
```

**Update Record**
```http
PUT /api/records/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Updated Title",
  "content": "Updated content..."
}

Response: 200 OK
```

**Delete Record**
```http
DELETE /api/records/1
Authorization: Bearer {token}

Response: 204 No Content
```

## Testing the Application

### Frontend Testing

1. **Login/Register Flow**
   - Navigate to http://localhost:5173
   - Register with a new account
   - Verify account creation message
   - Login with credentials
   - Verify redirect to records page

2. **Records CRUD Operations**
   - **Create**: Click "New Record", fill form, submit
   - **Read**: View records in grid layout
   - **Update**: Click edit icon, modify content, save
   - **Delete**: Click delete icon, confirm deletion

3. **Error Handling**
   - Try login with wrong password (should show error)
   - Try registering with existing email (should show error)
   - Try creating record with empty title (should show validation error)
   - Try accessing records without authentication (should redirect to login)

4. **Responsive Testing**
   - Resize browser to tablet size (768px)
   - Verify layout adapts correctly
   - Resize to mobile size (375px)
   - Verify touch-friendly interface

5. **UI/UX Testing**
   - Verify loading spinners appear during API calls
   - Check success notification appears after create/update/delete
   - Verify error messages are clear and helpful
   - Test keyboard navigation (Tab, Enter, Escape)

### Backend Testing

1. **Unit Tests**
```bash
cd BallastLaneTest.Tests
dotnet test
```

2. **Manual API Testing with curl**

Register:
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test@example.com","password":"TestPass123"}'
```

Login:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"TestPass123"}'
```

Create Record (replace TOKEN with actual token):
```bash
curl -X POST http://localhost:5000/api/records \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Test Record","content":"This is a test"}'
```

3. **API Documentation**

When backend is running, visit:
- OpenAPI/Swagger: `http://localhost:5000/openapi/v1.json`
- Swagger UI: `http://localhost:5000/swagger` (if enabled)

## Troubleshooting

### Backend Issues

**Migration Errors**
```bash
# Remove existing database
dotnet ef database drop --force

# Reapply migrations
dotnet ef database update
```

**Port Conflicts**
If port 5000 is in use:
```bash
# Change in launchSettings.json
# Look for "applicationUrl": "https://localhost:7000;http://localhost:5000"
```

**CORS Issues**
The backend has CORS configured in `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", builder =>
		builder.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());
});
```

### Frontend Issues

**API Connection Failed**
1. Verify backend is running: `http://localhost:5000/api/records` should prompt login
2. Check .env.development has correct VITE_API_URL
3. Clear localStorage: `localStorage.clear()` in browser console

**Build Errors**
```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
npm run build
```

**Blank Page After Login**
1. Open browser DevTools (F12)
2. Check Console tab for JavaScript errors
3. Check Network tab for failed API calls
4. Verify token is stored: `localStorage.getItem('authToken')`

## Performance Monitoring

### Backend
- Enable SQL query logging in `appsettings.Development.json`
- Use Application Insights for production monitoring
- Monitor response times in browser DevTools

### Frontend
- Use Lighthouse in Chrome DevTools
- Check bundle size: `npm run build` shows gzip sizes
- Monitor network requests in DevTools Network tab

## Security Considerations

1. **JWT Tokens**
   - Stored in localStorage (consider moving to httpOnly cookies in production)
   - Included in Authorization header as Bearer token
   - Validated on backend

2. **HTTPS**
   - Enable in production
   - Update VITE_API_URL to use https://

3. **Input Validation**
   - Frontend validates form inputs
   - Backend validates all incoming data
   - SQL injection prevention via Entity Framework

4. **Authentication**
   - Passwords hashed using secure algorithms
   - Token expiration should be configured
   - Implement refresh tokens for long sessions

## Deployment Checklist

- [ ] Backend built and tested
- [ ] Frontend built and tested
- [ ] Environment variables configured (HTTPS URLs, secrets)
- [ ] Database migrations applied
- [ ] SSL/TLS certificates installed
- [ ] CORS properly configured
- [ ] Logging enabled
- [ ] Error monitoring configured
- [ ] Backups scheduled
- [ ] Load testing completed

## Additional Resources

- [React Documentation](https://react.dev)
- [TypeScript Handbook](https://www.typescriptlang.org/docs)
- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core)
- [React Router](https://reactrouter.com)
- [Vite Guide](https://vitejs.dev/guide)
