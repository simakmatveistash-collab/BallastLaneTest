# Records Manager Frontend

A modern, responsive React frontend for managing user records with authentication.

## Overview

This is a professional full-stack frontend application built with:
- **React 19.2.1** - Latest UI library
- **TypeScript** - Type-safe development
- **React Router v7** - Client-side routing
- **Vite** - Lightning-fast build tool
- **Responsive CSS** - Mobile-first design

## Features

### Authentication
- User registration with email and password
- User login with JWT token support
- Persistent session handling via localStorage
- Protected routes for authenticated users
- Automatic redirection based on auth state

### Records Management (CRUD)
- **Create**: Add new records with title and content
- **Read**: View all user records in a responsive grid layout
- **Update**: Edit existing records via modal dialog
- **Delete**: Remove records with confirmation
- Real-time loading and error states
- Success/error notifications

### User Experience
- Clean, professional UI design
- Responsive layout (desktop, tablet, mobile)
- Loading spinners and skeleton states
- Form validation with helpful error messages
- Smooth animations and transitions
- Accessibility features (ARIA labels, keyboard navigation)

## Project Structure

```
frontend/
├── src/
│   ├── components/           # Reusable React components
│   │   ├── Header.tsx        # Navigation header
│   │   ├── ProtectedRoute.tsx # Auth guard component
│   │   ├── RecordForm.tsx    # Create/edit modal dialog
│   │   └── RecordsList.tsx   # Records grid display
│   ├── context/              # React Context for state management
│   │   └── AuthContext.tsx   # Global auth state
│   ├── pages/                # Page components
│   │   ├── LoginPage.tsx     # Login form
│   │   ├── RegisterPage.tsx  # Registration form
│   │   └── RecordsPage.tsx   # Main records management page
│   ├── services/             # API communication layer
│   │   ├── api.ts            # HTTP client with auth
│   │   ├── authService.ts    # Auth business logic
│   │   └── recordService.ts  # Records API operations
│   ├── styles/               # Component-scoped stylesheets
│   │   ├── Auth.css
│   │   ├── Header.css
│   │   ├── Records.css
│   │   ├── RecordForm.css
│   │   └── RecordsList.css
│   ├── App.tsx               # Root component with routing
│   ├── App.css               # Global styles
│   ├── index.css             # Base styles
│   └── main.tsx              # Entry point
├── package.json
├── tsconfig.json
├── vite.config.ts
└── index.html
```

## Installation

### Prerequisites
- Node.js >= 20.19.0 or >= 22.12.0
- npm >= 10.0.0

### Setup

1. Install dependencies:
```bash
cd frontend
npm install
```

2. Configure the API URL:
```bash
# Create .env.development file
VITE_API_URL=http://localhost:5000
```

3. Start development server:
```bash
npm run dev
```

4. Build for production:
```bash
npm run build
```

## Available Scripts

- `npm run dev` - Start development server with hot reload
- `npm run build` - Build optimized production bundle
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint code quality checks

## API Integration

The frontend communicates with the backend API at the following endpoints:

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login with credentials

### Records (Requires Authentication)
- `GET /api/records` - Get all user records
- `GET /api/records/{id}` - Get specific record
- `POST /api/records` - Create new record
- `PUT /api/records/{id}` - Update record
- `DELETE /api/records/{id}` - Delete record

## Authentication Flow

1. **User Registration**: Submit name, email, password → Account created
2. **User Login**: Submit email, password → JWT token received and stored
3. **Protected Routes**: Token automatically attached to API requests
4. **Session Persistence**: Token kept in localStorage across sessions
5. **Logout**: Token cleared, redirected to login page

## Component Architecture

### Header Component
- Displays application branding
- Shows logged-in user name
- Logout button to end session
- Sticky positioning for always-visible navigation

### ProtectedRoute Component
- Wraps routes requiring authentication
- Redirects to login if not authenticated
- Ensures only authorized users access protected pages

### RecordForm Component
- Reusable modal dialog for create/edit operations
- Form validation with error display
- Handles both new record creation and existing record updates
- Closes backdrop on successful submission

### RecordsList Component
- Responsive grid layout for records
- Card-based design with hover effects
- Quick action buttons (edit, delete)
- Timestamps for created and updated dates
- Content preview with text truncation

### RecordsPage Component
- Main page for record management
- Orchestrates CRUD operations
- Loading, error, and empty states
- Success notifications
- Modal form integration

## Styling System

The application uses a comprehensive CSS variable system for consistent design:

### Color Palette
- Primary: `#2563eb` (Blue)
- Secondary: `#64748b` (Slate)
- Error: `#ef4444` (Red)
- Success: `#10b981` (Green)

### Spacing Scale
- xs: 0.25rem
- sm: 0.5rem
- md: 1rem
- lg: 1.5rem
- xl: 2rem
- 2xl: 3rem

### Responsive Breakpoints
- Desktop: Full width (no breakpoint)
- Tablet: max-width 768px
- Mobile: max-width 480px

## Performance

Build outputs (production):
- Main JS bundle: ~248 KB (78 KB gzip)
- CSS bundle: ~13.5 KB (3.1 KB gzip)
- HTML: ~0.47 KB (0.3 KB gzip)

## Browser Support

- Chrome/Edge latest
- Firefox latest
- Safari latest
- Mobile browsers (iOS Safari, Chrome Mobile)

## Error Handling

The application includes comprehensive error handling:
- Network error messages
- Form validation errors
- API error responses
- Graceful fallbacks
- User-friendly error notifications

## Loading States

- Spinner animation during data fetches
- Disabled form buttons while submitting
- Loading text feedback to users
- Skeleton placeholders for records

## Accessibility Features

- Semantic HTML (header, main, article, etc.)
- ARIA labels and descriptions
- Keyboard navigation support
- Focus indicators on interactive elements
- Alt text for images
- Reduced motion support for animations
- High contrast colors for readability

## Development Best Practices

1. **Component Composition**: Small, focused, reusable components
2. **Type Safety**: Full TypeScript coverage
3. **Service Layer**: Centralized API communication
4. **State Management**: Context API for global state
5. **Error Boundaries**: Graceful error handling
6. **Code Organization**: Clear folder structure by feature
7. **Responsive Design**: Mobile-first approach
8. **Accessibility**: WCAG compliance

## Future Enhancements

- [ ] Token refresh mechanism
- [ ] User profile management
- [ ] Search and filter records
- [ ] Record categories/tags
- [ ] Dark mode support
- [ ] Collaborative editing
- [ ] Record versioning/history
- [ ] Export records to PDF/CSV

## Troubleshooting

### Build Fails
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm install
npm run build
```

### API Connection Issues
- Verify backend is running on `http://localhost:5000`
- Check `.env.development` has correct `VITE_API_URL`
- Ensure CORS is enabled in backend

### Hot Reload Not Working
```bash
# Restart dev server
npm run dev
```

## License

This project is part of the BallastLaneTest application.
