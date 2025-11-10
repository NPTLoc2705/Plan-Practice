// src/components/APIService/PaymentAPI.js
const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}/Payment`;

/**
 * Payment API - handles payment operations
 */
class PaymentAPI {
  /**
   * Create a coin payment (requires authentication)
   * @param {number} packageId - The package ID to purchase
   * @param {string} description - Optional payment description
   * @returns {Promise<object>} Payment creation response with checkout URL
   */
  static async createCoinPayment(packageId, description = '') {
    const token = localStorage.getItem('token');
    
    if (!token) {
      throw new Error('User not authenticated');
    }

    const response = await fetch(`${API_BASE_URL}/create-coin-payment`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify({
        packageId,
        description,
      }),
    });

    const result = await response.json();
    
    if (!response.ok) {
      throw new Error(result.message || 'Failed to create payment');
    }
    
    return result;
  }

  /**
   * Get payment status by order code
   * @param {number} orderCode - The order code to check
   * @returns {Promise<object>} Payment status details
   */
  static async getPaymentStatus(orderCode) {
    const token = localStorage.getItem('token');
    
    if (!token) {
      throw new Error('User not authenticated');
    }

    const response = await fetch(`${API_BASE_URL}/status/${orderCode}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    const result = await response.json();
    
    if (!response.ok) {
      throw new Error(result.message || 'Failed to get payment status');
    }
    
    return result;
  }

  /**
   * Get payment by PayOS payment link ID
   * @param {string} paymentLinkId - The PayOS payment link ID
   * @returns {Promise<object>} Payment details
   */
  static async getPaymentByLinkId(paymentLinkId) {
    const token = localStorage.getItem('token');
    
    if (!token) {
      throw new Error('User not authenticated');
    }

    const response = await fetch(`${API_BASE_URL}/by-link/${paymentLinkId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    const result = await response.json();
    
    if (!response.ok) {
      throw new Error(result.message || 'Failed to get payment by link ID');
    }
    
    return result;
  }

  /**
   * Get user's payment history
   * @returns {Promise<Array>} List of user's payments
   */
  static async getPaymentHistory() {
    const token = localStorage.getItem('token');
    
    if (!token) {
      throw new Error('User not authenticated');
    }

    const response = await fetch(`${API_BASE_URL}/history`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    const result = await response.json();
    
    if (!response.ok) {
      throw new Error(result.message || 'Failed to get payment history');
    }
    
    return result;
  }

  /**
   * Sync pending payments and auto-upgrade if successful
   * @returns {Promise<object>} Sync result with upgrade count
   */
  static async syncPendingPayments() {
    const token = localStorage.getItem('token');
    
    if (!token) {
      throw new Error('User not authenticated');
    }

    const response = await fetch(`${API_BASE_URL}/sync-pending`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    const result = await response.json();
    
    if (!response.ok) {
      throw new Error(result.message || 'Failed to sync payments');
    }
    
    return result;
  }

  /**
   * Manually upgrade to VIP if payment exists
   * @returns {Promise<object>} Upgrade result
   */
  static async upgradeToVip() {
    const token = localStorage.getItem('token');
    
    if (!token) {
      throw new Error('User not authenticated');
    }

    const response = await fetch(`${API_BASE_URL}/upgrade-to-vip`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    const result = await response.json();
    
    if (!response.ok) {
      throw new Error(result.message || 'Failed to upgrade to VIP');
    }
    
    return result;
  }
}

export default PaymentAPI;