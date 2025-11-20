const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}`;

class QuizOTPAPI {
    static getAuthHeaders() {
        const token = localStorage.getItem('token');
        return {
            'Content-Type': 'application/json',
            ...(token && { 'Authorization': `Bearer ${token}` })
        };
    }

    static async parseResponse(response) {
        try {
            const text = await response.text();
            if (!text) {
                return { success: false, data: null, message: 'Empty response received' };
            }
            return JSON.parse(text);
        } catch (e) {
            console.error('JSON Parse Error:', e);
            throw new Error('Invalid response format from server');
        }
    }

    static handleUnauthorized() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        if (window.location.pathname !== '/login') {
            window.location.href = '/login';
        }
    }

    // ===== TEACHER METHODS =====

    static async generateOTP(otpData) {
        const response = await fetch(`${API_BASE_URL}/QuizOTP/generate`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(otpData)
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to generate OTP');
        }

        return await this.parseResponse(response);
    }

    static async getMyOTPs() {
        const response = await fetch(`${API_BASE_URL}/QuizOTP/my-otps`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch OTPs');
        }

        return await this.parseResponse(response);
    }

    // static async getQuizOTPs(quizId) {
    //     const response = await fetch(`${API_BASE_URL}/QuizOTP/quiz/${quizId}`, {
    //         method: 'GET',
    //         headers: this.getAuthHeaders()
    //     });

    //     if (!response.ok) {
    //         if (response.status === 401) {
    //             this.handleUnauthorized();
    //         }
    //         const result = await this.parseResponse(response);
    //         throw new Error(result.message || 'Failed to fetch quiz OTPs');
    //     }

    //     return await this.parseResponse(response);
    // }

    static async revokeOTP(otpId) {
        const response = await fetch(`${API_BASE_URL}/QuizOTP/${otpId}`, {
            method: 'DELETE',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 404) {
                throw new Error('OTP not found');
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to revoke OTP');
        }

        return await this.parseResponse(response);
    }

    static async extendOTP(otpId, additionalMinutes) {
        const response = await fetch(`${API_BASE_URL}/QuizOTP/${otpId}/extend`, {
            method: 'PATCH',
            headers: this.getAuthHeaders(),
            body: JSON.stringify({ additionalMinutes })
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to extend OTP');
        }

        return await this.parseResponse(response);
    }

    static async regenerateOTP(otpId) {
    const response = await fetch(`${API_BASE_URL}/QuizOTP/${otpId}/regenerate`, {
        method: 'POST',
        headers: this.getAuthHeaders()
    });

    if (!response.ok) {
        if (response.status === 401) {
            this.handleUnauthorized();
        }
        const result = await this.parseResponse(response);
        throw new Error(result.message || 'Failed to regenerate OTP');
    }

    const result = await this.parseResponse(response);
    
    // Normalize the response data
    if (result.success && result.data) {
        result.data = {
            ...result.data,
            otpCode: result.data.otpCode || result.data.OTPCode,
            quizTitle: result.data.quizTitle || result.data.QuizTitle,
            expiresAt: result.data.expiresAt || result.data.ExpiresAt,
            maxUsage: result.data.maxUsage || result.data.MaxUsage
        };
    }
    
    return result;
}

    static async getOTPAccessLogs(otpId) {
        const response = await fetch(`${API_BASE_URL}/QuizOTP/${otpId}/logs`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch access logs');
        }

        return await this.parseResponse(response);
    }

    // ===== STUDENT METHODS =====

    static async validateOTP(otpCode) {
        const response = await fetch(`${API_BASE_URL}/QuizOTP/validate`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify({ otpCode })
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Invalid OTP code');
        }

        return await this.parseResponse(response);
    }

    static async getQuizByOTP(otpCode) {
        const response = await fetch(`${API_BASE_URL}/QuizOTP/take/${otpCode}`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to retrieve quiz');
        }

        return await this.parseResponse(response);
    }

    // ===== HELPER METHODS =====

    static formatOTPData(quizId, expiryMinutes, maxUsage, allowMultipleAttempts) {
        return {
            quizId: parseInt(quizId),
            expiryMinutes: parseInt(expiryMinutes),
            maxUsage: maxUsage ? parseInt(maxUsage) : null,
            allowMultipleAttempts: Boolean(allowMultipleAttempts)
        };
    }

    static isOTPExpired(expiresAt) {
        return new Date(expiresAt) < new Date();
    }

    static isOTPActive(otp) {
        return otp.isActive && !this.isOTPExpired(otp.expiresAt);
    }

    static formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    static getTimeRemaining(expiresAt) {
        const now = new Date();
        const expiry = new Date(expiresAt);
        const diff = expiry - now;

        if (diff <= 0) return 'Expired';

        const hours = Math.floor(diff / (1000 * 60 * 60));
        const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

        if (hours > 24) {
            const days = Math.floor(hours / 24);
            return `${days}d ${hours % 24}h remaining`;
        } else if (hours > 0) {
            return `${hours}h ${minutes}m remaining`;
        } else {
            return `${minutes}m remaining`;
        }
    }

    static getOTPStatus(otp) {
        if (!otp.isActive) {
            return { text: 'Revoked', class: 'badgeInactive' };
        }
        if (this.isOTPExpired(otp.expiresAt)) {
            return { text: 'Expired', class: 'badgeExpired' };
        }
        if (otp.maxUsage && otp.usageCount >= otp.maxUsage) {
            return { text: 'Limit Reached', class: 'badgeExpired' };
        }
        return { text: 'Active', class: 'badgeActive' };
    }

    static async copyToClipboard(text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (error) {
            console.error('Failed to copy:', error);
            return false;
        }
    }

    static formatUsage(usageCount, maxUsage) {
        return `${usageCount} / ${maxUsage || '∞'}`;
    }
}

export default QuizOTPAPI;