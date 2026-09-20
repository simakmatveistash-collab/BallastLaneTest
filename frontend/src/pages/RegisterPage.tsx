/**
 * RegisterPage Component
 * User registration form with name, email, and password inputs
 */

import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../styles/Auth.css';

export function RegisterPage() {
  const navigate = useNavigate();
  const { register, isLoading, error, clearError } = useAuth();
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [validationError, setValidationError] = useState('');

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    setValidationError('');
  };

  const validateForm = (): boolean => {
    if (!formData.name.trim()) {
      setValidationError('Name is required');
      return false;
    }
    if (!formData.email.trim()) {
      setValidationError('Email is required');
      return false;
    }
    if (!formData.email.includes('@')) {
      setValidationError('Invalid email format');
      return false;
    }
    if (!formData.password) {
      setValidationError('Password is required');
      return false;
    }
    if (formData.password.length < 6) {
      setValidationError('Password must be at least 6 characters');
      return false;
    }
    if (formData.password !== formData.confirmPassword) {
      setValidationError('Passwords do not match');
      return false;
    }
    return true;
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      clearError();
      await register(formData.name, formData.email, formData.password);
      // Redirect to login with a message (can be implemented with search params)
      navigate('/login?registered=true');
    } catch {
      // Error is handled by context
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h1 className="auth-title">Create Account</h1>
        <p className="auth-subtitle">Sign up to get started with Records Manager</p>

        {(error || validationError) && (
          <div className="error-message" role="alert">
            {error || validationError}
          </div>
        )}

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="form-group">
            <label htmlFor="name" className="form-label">Full Name</label>
            <input
              id="name"
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              placeholder="John Doe"
              disabled={isLoading}
              className="form-input"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="email" className="form-label">Email Address</label>
            <input
              id="email"
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="you@example.com"
              disabled={isLoading}
              className="form-input"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password" className="form-label">Password</label>
            <input
              id="password"
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
              disabled={isLoading}
              className="form-input"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword" className="form-label">Confirm Password</label>
            <input
              id="confirmPassword"
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              placeholder="••••••••"
              disabled={isLoading}
              className="form-input"
              required
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="btn btn-primary btn-block"
          >
            {isLoading ? 'Creating account...' : 'Create Account'}
          </button>
        </form>

        <p className="auth-footer">
          Already have an account?{' '}
          <Link to="/login" className="auth-link">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
