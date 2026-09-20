/**
 * App Component
 * Root component with routing and auth context provider
 */

import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { Header } from './components/Header';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { RecordsPage } from './pages/RecordsPage';
import './App.css';

function AppContent() {
  const { isAuthenticated } = useAuth();

  return (
    <div className="app-container">
      {isAuthenticated && <Header />}

      <main className="main-content">
        <Routes>
          {/* Public Routes */}
          <Route
            path="/login"
            element={isAuthenticated ? <Navigate to="/records" replace /> : <LoginPage />}
          />
          <Route
            path="/register"
            element={isAuthenticated ? <Navigate to="/records" replace /> : <RegisterPage />}
          />

          {/* Protected Routes */}
          <Route
            path="/records"
            element={
              <ProtectedRoute>
                <RecordsPage />
              </ProtectedRoute>
            }
          />

          {/* Default Route */}
          <Route
            path="/"
            element={isAuthenticated ? <Navigate to="/records" replace /> : <Navigate to="/login" replace />}
          />

          {/* 404 Route */}
          <Route
            path="*"
            element={<Navigate to={isAuthenticated ? '/records' : '/login'} replace />}
          />
        </Routes>
      </main>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
