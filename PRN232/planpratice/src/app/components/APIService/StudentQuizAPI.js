const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}`;

class QuizAPI {
    static getAuthHeaders() {
        const token = localStorage.getItem('token');
        return {
            'Content-Type': 'application/json',
            ...(token && { 'Authorization': `Bearer ${token}` })
        };
    }

    static async getAvailableQuizzes() {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/available`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const text = await response.text();
            let errorMessage;
            try {
                const result = JSON.parse(text);
                errorMessage = result.message;
            } catch (e) {
                errorMessage = text || 'Failed to fetch available quizzes';
            }
            throw new Error(errorMessage);
        }

        try {
            const text = await response.text();
            if (!text) {
                return { success: false, data: [], message: 'Empty response received' };
            }
            const result = JSON.parse(text);
            return result;
        } catch (e) {
            console.error('JSON Parse Error:', e);
            throw new Error('Invalid response format from server');
        }
    }

    // Update other API methods with similar error handling
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

    static async getQuizForTaking(quizId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/${quizId}`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        const result = await response.json();

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 404) {
                throw new Error('Quiz not found');
            }
            throw new Error(result.message || 'Failed to fetch quiz details');
        }

        return result;
    }

    static async checkQuizAttempt(quizId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/${quizId}/check-attempt`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        const result = await response.json();

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            throw new Error(result.message || 'Failed to check quiz attempt');
        }

        return result;
    }

    static async submitQuiz(quizData) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/submit`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(quizData)
        });

        const result = await response.json();

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 400) {
                throw new Error(result.message || 'Invalid quiz submission');
            }
            throw new Error(result.message || 'Failed to submit quiz');
        }

        return result;
    }

    static async getQuizResult(resultId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/result/${resultId}`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        const result = await response.json();

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 404) {
                throw new Error('Result not found or you do not have permission to view this result');
            }
            throw new Error(result.message || 'Failed to fetch quiz result');
        }

        return result;
    }

    static async getQuizHistory() {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/history`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        const result = await response.json();

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            throw new Error(result.message || 'Failed to fetch quiz history');
        }

        return result;
    }

    static async getQuizStatistics(quizId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/${quizId}/statistics`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        const result = await response.json();

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 404) {
                throw new Error('Quiz not found');
            }
            throw new Error(result.message || 'Failed to fetch quiz statistics');
        }

        return result;
    }

    static handleUnauthorized() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        if (window.location.pathname !== '/login') {
            window.location.href = '/login';
        }
    }

    // Helper methods cho quiz data
    static formatQuizData(quizId, answers) {
        return {
            quizId: parseInt(quizId),
            answers: answers.map(answer => ({
                questionId: parseInt(answer.questionId),
                answerId: parseInt(answer.answerId)
            }))
        };
    }

    // Calculate percentage
    static calculatePercentage(score, total) {
        if (total === 0) return 0;
        return Math.round((score / total) * 100 * 100) / 100; // Round to 2 decimal places
    }

    // Format time display
    static formatTime(seconds) {
        const hours = Math.floor(seconds / 3600);
        const minutes = Math.floor((seconds % 3600) / 60);
        const secs = seconds % 60;

        if (hours > 0) {
            return `${hours}h ${minutes}m ${secs}s`;
        } else if (minutes > 0) {
            return `${minutes}m ${secs}s`;
        } else {
            return `${secs}s`;
        }
    }

    // Format date
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

    // Get score color class based on percentage
    static getScoreColorClass(percentage) {
        if (percentage >= 80) return 'text-success';
        if (percentage >= 60) return 'text-info';
        if (percentage >= 40) return 'text-warning';
        return 'text-danger';
    }

    // Check if quiz is expired (if you add time limits)
    static isQuizExpired(startTime, timeLimit) {
        const now = new Date();
        const start = new Date(startTime);
        const elapsedMinutes = (now - start) / (1000 * 60);
        return elapsedMinutes > timeLimit;
    }
}

export default QuizAPI;