// src/services/adminAPI.js
const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}/admin`;

class AdminAPI {
  // Add these methods:
  static async getStudents() {
    return this.getUsers('Student');
  }

  static async getTeachers() {
    return this.getUsers('Teacher');
  }

  static async getUsers(role = null) {
    const token = localStorage.getItem('token');
    let url = `${API_BASE_URL}/users`;
    if (role) {
      url += `?role=${role}`;
    }

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to fetch users');
    return result;
  }

  static async getPaginatedUsers(pageNumber = 1, pageSize = 10, searchTerm = '', role = null) {
    const token = localStorage.getItem('token');
    let url = `${API_BASE_URL}/users/paginated?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    
    if (searchTerm) {
      url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    if (role) {
      url += `&role=${role}`;
    }

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to fetch users');
    return result;
  }

  static async banUser(userId) {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/users/${userId}/ban`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to ban user');
    return result;
  }

  static async unbanUser(userId) {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/users/${userId}/unban`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to unban user');
    return result;
  }

  static async updateUser(userId, userData) {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/users/${userId}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(userData)
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to update user');
    return result;
  }
}

export default AdminAPI;