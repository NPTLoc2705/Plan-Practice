// src/services/UserAPI.js
const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}/user`;

class UserAPI {
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

  static async updateProfile(updateData) {
    const token = this.getToken();
    if (!token) {
      throw new Error('No authentication token found');
    }

    // Map frontend camelCase to backend PascalCase
    const backendData = {
      UserName: updateData.userName !== undefined ? updateData.userName : '',
      Phone: updateData.phone !== undefined ? updateData.phone : '',
      OldPassword: updateData.oldPassword || '',
      NewPassword: updateData.newPassword || ''
    };

    const response = await fetch(`${API_BASE_URL}/profile`, {
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

  static async updateTeacherRole(email) {
    const response = await fetch(`${API_BASE_URL}/update-teacher?email=${encodeURIComponent(email)}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      const result = await response.json();
      throw new Error(result.message || 'Failed to update teacher role');
    }

    return await response.json();
  }

  // Helper method to get token from localStorage
  static getToken() {
    return localStorage.getItem('token');
  }

  // Helper method to get user from localStorage
  static getUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }
}

export { UserAPI };