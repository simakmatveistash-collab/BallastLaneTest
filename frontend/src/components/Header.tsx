/**
 * Header Component
 * Navigation header with user info and logout button
 */

import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../styles/Header.css';

export function Header() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="app-header">
      <div className="header-container">
        <div className="header-content">
          <h1 className="app-logo">
            <span className="logo-icon">📝</span>
            Records Manager
          </h1>
          {user && (
            <nav className="header-nav">
              <p className="user-info">
                Welcome, <span className="user-name">{user.name}</span>
              </p>
              <button
                onClick={handleLogout}
                className="btn btn-secondary"
              >
                Sign Out
              </button>
            </nav>
          )}
        </div>
      </div>
    </header>
  );
}
