# Quick Start Guide

## 🚀 Get Started in 5 Minutes

### Prerequisites
- ✅ .NET 10 SDK installed
- ✅ Node.js 20+ installed
- ✅ SQL Server/LocalDB available

---

## Step 1: Backend Setup (2 minutes)

```bash
# Navigate to backend
cd BallastLaneTest.Server

# Apply database migrations
dotnet ef database update

# Start the API server
dotnet run

# Backend running at: http://localhost:5000
```

---

## Step 2: Frontend Setup (2 minutes)

```bash
# Navigate to frontend
cd frontend

# Install dependencies
npm install

# Start development server
npm run dev

# Frontend running at: http://localhost:5173
```

---

## Step 3: Test the Application (1 minute)

### In your browser, go to: **http://localhost:5173**

1. **Register a new account**
   - Click "Create one"
   - Fill in name, email, password
   - Click "Create Account"

2. **Login**
   - Use your credentials
   - Click "Sign In"

3. **Create a record**
   - Click "+ New Record"
   - Enter a title and content
   - Click "Create Record"

4. **Test CRUD operations**
   - Edit: Click the ✎ button
   - Delete: Click the 🗑 button
   - View: Cards show your records in a grid

---

## Common Tasks

### View API Documentation
Backend Swagger UI (if enabled):
```
http://localhost:5000/swagger
```

### Stop Services
- Backend: Press `Ctrl+C` in .NET terminal
- Frontend: Press `Ctrl+C` in npm terminal

### Clear Application Data
```bash
# Option 1: Clear login session in browser
# Open DevTools (F12) → Application → Storage → Clear All

# Option 2: Reset database
dotnet ef database drop --force
dotnet ef database update
```

### Build Frontend for Production
```bash
cd frontend
npm run build
# Output in: frontend/dist/
```

---

## 📝 Default Test Credentials

After running `dotnet ef database update`, the database may be seeded with:
- Email: `test@example.com`
- Password: `Password123`

---

## File Locations

- **Backend API**: `http://localhost:5000`
- **Frontend App**: `http://localhost:5173`
- **API Endpoints**: `/api/auth/*`, `/api/records/*`
- **Frontend Source**: `./frontend/src/`
- **Backend Source**: `./BallastLaneTest.Server/`

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Database connection failed" | Ensure LocalDB is running: `sqllocaldb start` |
| "Port 5000 already in use" | Change port in `launchSettings.json` |
| "CORS error" | Backend CORS already configured, check URL is correct |
| "Frontend can't connect to API" | Verify `VITE_API_URL` in `.env.development` |
| "npm audit vulnerabilities" | Run `npm audit fix` (optional for development) |

---

## Next Steps

- Read [`FULLSTACK_SETUP.md`](FULLSTACK_SETUP.md) for detailed configuration
- Read [`frontend/README.md`](frontend/README.md) for frontend documentation
- Read [`FRONTEND_IMPLEMENTATION.md`](FRONTEND_IMPLEMENTATION.md) for architecture overview

---

## 🎉 You're Ready!

Your full-stack application is now running. Start creating records!

**Need help?** Check the detailed guides in the repository root.
