// src/services/PackageAPI.js
const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}/Package`;

/**
 * Package API – thin wrapper around the backend Package controller
 */
class PackageAPI {
  /**
   * GET /api/Package/{id}
   * @param {number} id – package id
   * @returns {Promise<object>} single package (or throws)
   */
  static async getById(id) {
    if (!id || id <= 0) throw new Error('Invalid package id');

    const response = await fetch(`${API_BASE_URL}/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        // No auth required for public packages
      },
    });

    const result = await response.json();
    if (!response.ok) {
      throw new Error(result.message || `Failed to fetch package ${id}`);
    }
    return result; // { id, name, price, description, features?, ... }
  }

  /**
   * GET /api/Package
   * @returns {Promise<Array<object>>} list of active packages
   */
  static async getAll() {
    const response = await fetch(API_BASE_URL, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    const result = await response.json();
    if (!response.ok) {
      throw new Error(result.message || 'Failed to fetch packages');
    }
    return result; // array of packages
  }
}

export default PackageAPI;