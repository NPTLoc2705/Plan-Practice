// src/services/authAPI.js
const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}/auth`;
const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID;

class AuthAPI {
  static async register(data) {
    const response = await fetch(`${API_BASE_URL}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Registration failed');
    return result;
  }

  static async verifyRegistration(data) {
    const response = await fetch(`${API_BASE_URL}/registration/verification`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Verification failed');
    return result;
  }

  static async resendOTP(email) {
    const response = await fetch(`${API_BASE_URL}/registration/otp/resend`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email })
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to resend OTP');
    return result;
  }

  static async login(credentials) {
    const response = await fetch(`${API_BASE_URL}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(credentials)
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Login failed');
    
    if (result.token) {
      localStorage.setItem('token', result.token);
      localStorage.setItem('user', JSON.stringify(result.user));
    }
    
    return result;
  }

  static async googleLogin(idToken) {
    const response = await fetch(`${API_BASE_URL}/google-login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ 
        idToken: idToken  
      })
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Google login failed');
    
    if (result.token) {
      localStorage.setItem('token', result.token);
      localStorage.setItem('user', JSON.stringify(result.user));
    }
    
    return result;
  }

  static logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  static isAuthenticated() {
    return !!localStorage.getItem('token');
  }

  static getUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  static getToken() {
    return localStorage.getItem('token');
  }

  static decodeToken() {
    const token = this.getToken();
    if (!token) return null;
    
    try {
      // JWT structure: header.payload.signature
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  static getUserRole() {
    const decoded = this.decodeToken();
    return decoded ? decoded.role : null;
  }

  static hasRole(requiredRole) {
    const userRole = this.getUserRole();
    if (!userRole) return false;
    
    // Case-insensitive comparison
    return userRole.toLowerCase() === requiredRole.toLowerCase();
  }

  static isTeacher() {
    return this.hasRole('Teacher');
  }

  static isStudent() {
    return this.hasRole('Student');
  }

  static isAdmin() {
    return this.hasRole('Admin');
  }
}

export { AuthAPI, GOOGLE_CLIENT_ID };