// src/services/adminAPI.js
const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}/Admin`;

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
  static async fetchWithAuth(url, options = {}) {
    const token = localStorage.getItem('token');
    const response = await fetch(url, {
      ...options,
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
        ...(options.headers || {}),
      },
    });
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to fetch');
    return result;
  }
  // Revenue methods
  static async getTotalRevenue(startDate = null, endDate = null) {
    let url = `${API_BASE_URL}/revenue`;
    if (startDate || endDate) {
      url += '?';
      if (startDate) url += `startDate=${startDate}&`;
      if (endDate) url += `endDate=${endDate}`;
    }
    return this.fetchWithAuth(url, { method: 'GET' });
  }

  static async getTodayRevenue() {
    return this.fetchWithAuth(`${API_BASE_URL}/revenue/today`, { method: 'GET' });
  }

  static async getMonthRevenue() {
    return this.fetchWithAuth(`${API_BASE_URL}/revenue/month`, { method: 'GET' });
  }

  static async getYearRevenue() {
    return this.fetchWithAuth(`${API_BASE_URL}/revenue/year`, { method: 'GET' });
  }

  static async getLast30DaysRevenue() {
    return this.fetchWithAuth(`${API_BASE_URL}/revenue/last30days`, { method: 'GET' });
  }

  // Transactions methods
  static async getPaidTransactions(startDate = null, endDate = null) {
    let url = `${API_BASE_URL}/transactions`;
    if (startDate || endDate) {
      url += '?';
      if (startDate) url += `startDate=${startDate}&`;
      if (endDate) url += `endDate=${endDate}`;
    }
    return this.fetchWithAuth(url, { method: 'GET' });
  }

  static async getPaginatedPaidTransactions(pageNumber = 1, pageSize = 10, startDate = null, endDate = null) {
    let url = `${API_BASE_URL}/transactions/paginated?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (startDate) url += `&startDate=${startDate}`;
    if (endDate) url += `&endDate=${endDate}`;
    return this.fetchWithAuth(url, { method: 'GET' });
  }

  // Package Management Methods
static async getAllPackages(includeInactive = true) {
    const url = `${API_BASE_URL}/packages?includeInactive=${includeInactive}`;
    return this.fetchWithAuth(url, { method: 'GET' });
  }

static async getPaginatedPackages(pageNumber = 1, pageSize = 10, searchTerm = '', isActive = null) {
    let url = `${API_BASE_URL}/packages/paginated?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) {
      url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    if (isActive !== null) {
      url += `&isActive=${isActive}`;
    }
    return this.fetchWithAuth(url, { method: 'GET' });
  }

 static async getPackageDetail(packageId) {
    return this.fetchWithAuth(`${API_BASE_URL}/packages/${packageId}`, { method: 'GET' });
  }

  static async createPackage(packageData) {
    return this.fetchWithAuth(`${API_BASE_URL}/packages`, {
      method: 'POST',
      body: JSON.stringify(packageData)
    });
  }

 static async updatePackage(packageId, packageData) {
    return this.fetchWithAuth(`${API_BASE_URL}/packages/${packageId}`, {
      method: 'PUT',
      body: JSON.stringify(packageData)
    });
  }

 static async deletePackage(packageId) {
    return this.fetchWithAuth(`${API_BASE_URL}/packages/${packageId}`, { 
      method: 'DELETE' 
    });
  }
}

export default AdminAPI;