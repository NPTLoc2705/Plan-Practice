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
    const response = await fetch(`${API_BASE_URL}/verify-registration`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Verification failed');
    return result;
  }

  static async resendOTP(email) {
    const response = await fetch(`${API_BASE_URL}/resend-registration-otp`, {
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

  static async updateProfile(updateData) {
    const token = this.getToken();
    if (!token) {
      throw new Error('No authentication token found');
    }

    // Map frontend camelCase to backend PascalCase
    // Always include all fields (empty strings are allowed by backend)
    const backendData = {
      UserName: updateData.userName !== undefined ? updateData.userName : '',
      Phone: updateData.phone !== undefined ? updateData.phone : '',
      OldPassword: updateData.oldPassword || '',
      NewPassword: updateData.newPassword || ''
    };

    const response = await fetch(`${API_BASE_URL}/update-profile`, {
      method: 'PUT',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(backendData)
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Profile update failed');
    
    // Update local storage with new user data if update was successful
    if (result.user) {
      const currentUser = this.getUser();
      const updatedUser = { 
        ...currentUser, 
        username: result.user.Username || result.user.username,
        phone: result.user.Phone || result.user.phone,
        email: result.user.Email || result.user.email,
        emailVerified: result.user.EmailVerified !== undefined ? result.user.EmailVerified : result.user.emailVerified
      };
      localStorage.setItem('user', JSON.stringify(updatedUser));
    }
    
    return result;
  }

  static async getProfile() {
    const token = this.getToken();
    if (!token) {
      throw new Error('No authentication token found');
    }

    const response = await fetch(`${API_BASE_URL}/profile`, {
      method: 'GET',
      headers: { 
        'Authorization': `Bearer ${token}`
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to fetch profile');
    
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
}

export { AuthAPI, GOOGLE_CLIENT_ID };