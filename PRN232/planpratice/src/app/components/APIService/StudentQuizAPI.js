const API_BASE_URL = `${import.meta.env.VITE_API_BASE_URL}`;

class QuizAPI {
    static formatQuizData(quizId, answers) {
        return {
            quizId: parseInt(quizId),
            answers: answers.map(answer => ({
                questionId: parseInt(answer.questionId),
                answerId: parseInt(answer.answerId)
            }))
        };
    }

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

    // ===== STUDENT METHODS =====
    
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

        return await this.parseResponse(response);
    }

    static async getQuizForTaking(quizId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/${quizId}`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 404) {
                throw new Error('Quiz not found');
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch quiz details');
        }

        return await this.parseResponse(response);
    }

    static async submitQuiz(quizData) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/submit`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(quizData)
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to submit quiz');
        }

        return await this.parseResponse(response);
    }

    static async checkQuizAttempt(quizId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/${quizId}/check-attempt`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to check quiz attempt');
        }

        return await this.parseResponse(response);
    }

    static async submitQuiz(quizData) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/submit`, {
            method: 'POST',
            headers: this.getAuthHeaders(),
            body: JSON.stringify(quizData)
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 400) {
                const result = await this.parseResponse(response);
                throw new Error(result.message || 'Invalid quiz submission');
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to submit quiz');
        }

        return await this.parseResponse(response);
    }

    static async getQuizResult(resultId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/result/${resultId}`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 404) {
                throw new Error('Result not found or you do not have permission to view this result');
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch quiz result');
        }

        return await this.parseResponse(response);
    }

    static async getQuizHistory() {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/history`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch quiz history');
        }

        return await this.parseResponse(response);
    }

    static async getQuizStatistics(quizId) {
        const response = await fetch(`${API_BASE_URL}/StudentQuiz/${quizId}/statistics`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });

        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            if (response.status === 404) {
                throw new Error('Quiz not found');
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch quiz statistics');
        }

        return await this.parseResponse(response);
    }

    // ===== TEACHER METHODS =====
    
    static async getTeacherDashboard() {
        const response = await fetch(`${API_BASE_URL}/quiz/teacher/me/dashboard`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });
        
        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch teacher dashboard');
        }
        
        return await this.parseResponse(response);
    }

    static async getTeacherQuizzes() {
        const response = await fetch(`${API_BASE_URL}/quiz/teacher/me`, {
            method: 'GET',
            headers: this.getAuthHeaders()
        });
        
        if (!response.ok) {
            if (response.status === 401) {
                this.handleUnauthorized();
            }
            const result = await this.parseResponse(response);
            throw new Error(result.message || 'Failed to fetch teacher quizzes');
        }
        
        return await this.parseResponse(response);
    }

    // ===== HELPER METHODS =====
    
    static formatQuizData(quizId, answers) {
        return {
            quizId: parseInt(quizId),
            answers: answers.map(answer => ({
                questionId: parseInt(answer.questionId),
                answerId: parseInt(answer.answerId)
            }))
        };
    }

    static calculatePercentage(score, total) {
        if (total === 0) return 0;
        return Math.round((score / total) * 100 * 100) / 100;
    }

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

    static getScoreColorClass(percentage) {
        if (percentage >= 80) return 'text-success';
        if (percentage >= 60) return 'text-info';
        if (percentage >= 40) return 'text-warning';
        return 'text-danger';
    }

    static isQuizExpired(startTime, timeLimit) {
        const now = new Date();
        const start = new Date(startTime);
        const elapsedMinutes = (now - start) / (1000 * 60);
        return elapsedMinutes > timeLimit;
    }
}

export default QuizAPI;