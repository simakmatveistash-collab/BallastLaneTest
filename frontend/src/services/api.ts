/**
 * API Client Service
 * Centralized HTTP client with authentication token handling
 */

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

export interface ApiError {
  message: string;
  status: number;
}

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  private async getAuthHeaders(): Promise<HeadersInit> {
    const token = localStorage.getItem('authToken');
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    return headers;
  }

  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      let errorMessage = `HTTP Error: ${response.status}`;

      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.error || errorMessage;
      } catch {
        // Use default error message if response body is not JSON
      }

      const error: ApiError = {
        message: errorMessage,
        status: response.status,
      };

      throw error;
    }

    if (response.status === 204) {
      return undefined as T;
    }

    return response.json() as Promise<T>;
  }

  async get<T>(endpoint: string): Promise<T> {
    const headers = await this.getAuthHeaders();
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'GET',
      headers,
    });

    return this.handleResponse<T>(response);
  }

  async post<T>(endpoint: string, body?: unknown): Promise<T> {
    const headers = await this.getAuthHeaders();
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'POST',
      headers,
      body: body ? JSON.stringify(body) : undefined,
    });

    return this.handleResponse<T>(response);
  }

  async put<T>(endpoint: string, body?: unknown): Promise<T> {
    const headers = await this.getAuthHeaders();
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'PUT',
      headers,
      body: body ? JSON.stringify(body) : undefined,
    });

    return this.handleResponse<T>(response);
  }

  async delete<T>(endpoint: string): Promise<T> {
    const headers = await this.getAuthHeaders();
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'DELETE',
      headers,
    });

    return this.handleResponse<T>(response);
  }

  setAuthToken(token: string | null): void {
    if (token) {
      localStorage.setItem('authToken', token);
    } else {
      localStorage.removeItem('authToken');
    }
  }

  getAuthToken(): string | null {
    return localStorage.getItem('authToken');
  }

  clearAuthToken(): void {
    localStorage.removeItem('authToken');
  }
}

export const apiClient = new ApiClient();
