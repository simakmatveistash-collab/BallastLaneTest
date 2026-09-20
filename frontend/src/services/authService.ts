/**
 * Authentication Service
 * Handles user login, registration, and token management
 */

import { apiClient } from './api';
import type { ApiError } from './api';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: UserDto;
}

export interface UserDto {
  id: number;
  name: string;
  email: string;
}

class AuthService {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    try {
      const response = await apiClient.post<any>('/api/auth/login', credentials);

      // API returns {userId, name, email, token}, map to {user, token}
      const mapped: LoginResponse = {
        token: response.token,
        user: {
          id: response.userId,
          name: response.name,
          email: response.email,
        },
      };
      apiClient.setAuthToken(mapped.token);
      return mapped;
    } catch (error) {
      throw this.handleAuthError(error);
    }
  }

  async register(userData: RegisterRequest): Promise<UserDto> {
    try {
      const response = await apiClient.post<UserDto>('/api/auth/register', userData);
      return response;
    } catch (error) {
      throw this.handleAuthError(error);
    }
  }

  logout(): void {
    apiClient.clearAuthToken();
  }

  getAuthToken(): string | null {
    return apiClient.getAuthToken();
  }

  isAuthenticated(): boolean {
    return !!this.getAuthToken();
  }

  private handleAuthError(error: unknown): Error {
    if (typeof error === 'object' && error !== null && 'message' in error) {
      const apiError = error as ApiError;

      if (apiError.status === 401) {
        return new Error('Invalid email or password');
      }

      if (apiError.status === 400) {
        return new Error(apiError.message || 'Invalid input');
      }

      return new Error(apiError.message || 'Authentication failed');
    }

    return error instanceof Error ? error : new Error('Authentication failed');
  }
}

export const authService = new AuthService();
