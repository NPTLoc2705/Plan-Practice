// src/services/QuizAPI.js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const headers = () => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${localStorage.getItem('token')}`,
});

export class QuizAPI {
    // 🟢 Get all quizzes (new method to replace teacher-specific endpoint)
    static async getAllQuizzes() {
        try {
            const response = await fetch(`${API_BASE_URL}/quiz`, {
                method: 'GET',
                headers: headers(),
            });

            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || 'Failed to fetch quizzes');
            }

            const result = await response.json();
            return Array.isArray(result) ? result : [];
        } catch (error) {
            console.error('Error in getAllQuizzes:', error);
            throw new Error('Failed to fetch quizzes');
        }
    }

    // 🟢 Get quizzes by lesson planner ID
    static async getQuizzesByLessonPlannerId(lessonPlannerId) {
        try {
            const response = await fetch(`${API_BASE_URL}/quiz`, {
                method: 'GET',
                headers: headers(),
            });

            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || 'Failed to fetch quizzes');
            }

            const result = await response.json();
            const allQuizzes = Array.isArray(result) ? result : [];

            // Filter quizzes by lesson planner ID
            return allQuizzes.filter(quiz => quiz.lessonPlannerId === lessonPlannerId);
        } catch (error) {
            console.error('Error in getQuizzesByLessonPlannerId:', error);
            throw new Error('Failed to fetch quizzes for lesson planner');
        }
    }

    // 🟢 Lấy danh sách quiz của teacher hiện tại
    static async getTeacherQuizzes() {
        try {
            const response = await fetch(`${API_BASE_URL}/quiz/teacher/me`, {
                method: 'GET',
                headers: headers(),
            });

            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || 'Failed to fetch quizzes');
            }

            const result = await response.json();

            // Transform the quiz data to match the component's expectations
            if (result.success && Array.isArray(result.data)) {
                return {
                    success: true,
                    data: result.data.map(quiz => ({
                        ...quiz,
                        totalQuestions: quiz.totalQuestion || 0 // Map from backend's totalQuestion to frontend's totalQuestions
                    })),
                    message: result.message
                };
            }

            return result;
        } catch (error) {
            console.error('Error in getTeacherQuizzes:', error);
            throw new Error('Failed to fetch quizzes');
        }
    }

    // 🟢 Tạo quiz mới
    static async createQuiz(data) {
        const response = await fetch(`${API_BASE_URL}/quiz`, {
            method: 'POST',
            headers: headers(),
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || 'Failed to create quiz');
        }

        return await response.json();
    }

    // 🟡 Cập nhật quiz
    static async updateQuiz(id, quizData) {
        const response = await fetch(`${API_BASE_URL}/quiz/${id}`, {
            method: 'PUT',
            headers: headers(),
            body: JSON.stringify(quizData)
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || 'Failed to update quiz');
        }

        // Handle empty response body from controller
        const text = await response.text();
        return text ? JSON.parse(text) : null;
    }

    // 🔴 Xóa quiz
    static async deleteQuiz(id) {
        const response = await fetch(`${API_BASE_URL}/quiz/${id}`, {
            method: 'DELETE',
            headers: headers(),
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || 'Failed to delete quiz');
        }

        // Handle empty response body from controller
        const text = await response.text();
        return text ? JSON.parse(text) : null;
    }

    // 🔍 Lấy chi tiết quiz theo id
    static async getQuizById(id) {
        const response = await fetch(`${API_BASE_URL}/quiz/${id}`, {
            method: 'GET',
            headers: headers(),
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || 'Failed to fetch quiz detail');
        }

        return await response.json();
    }

    // ========================= QUESTION =========================
    static async getQuestionsByQuizId(quizId) {
        const res = await fetch(`${API_BASE_URL}/question/quiz/${quizId}`, {
            method: 'GET',
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to fetch questions');
        return res.json();
    }

    static async updateQuestion(id, question) {
        const res = await fetch(`${API_BASE_URL}/question/${id}`, {
            method: 'PUT',
            headers: headers(),
            body: JSON.stringify(question),
        });
        if (!res.ok) throw new Error('Failed to update question');

        // Handle empty response body from controller
        const text = await res.text();
        return text ? JSON.parse(text) : null;
    }

    static async createQuestion(question) {
        const res = await fetch(`${API_BASE_URL}/question`, {
            method: 'POST',
            headers: headers(),
            body: JSON.stringify(question),
        });
        if (!res.ok) throw new Error('Failed to create question');
        return res.json();
    }

    static async deleteQuestion(questionId) {
        const res = await fetch(`${API_BASE_URL}/question/${questionId}`, {
            method: 'DELETE',
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to delete question');

        // Handle empty response body from controller
        const text = await res.text();
        return text ? JSON.parse(text) : null;
    }

    // ========================= ANSWER =========================
    static async getAnswersByQuestionId(questionId) {
        const res = await fetch(`${API_BASE_URL}/answer/question/${questionId}`, {
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to fetch answers for question');
        return res.json();
    }

    static async createAnswer(answer) {
        const res = await fetch(`${API_BASE_URL}/answer`, {
            method: 'POST',
            headers: headers(),
            body: JSON.stringify(answer),
        });
        if (!res.ok) throw new Error('Failed to create answer');
        return res.json();
    }

    static async updateAnswer(id, answer) {
        const res = await fetch(`${API_BASE_URL}/answer/${id}`, {
            method: 'PUT',
            headers: headers(),
            body: JSON.stringify(answer),
        });
        if (!res.ok) throw new Error('Failed to update answer');

        // Handle empty response body from controller
        const text = await res.text();
        return text ? JSON.parse(text) : null;
    }

    static async deleteAnswer(id) {
        const res = await fetch(`${API_BASE_URL}/answer/${id}`, {
            method: 'DELETE',
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to delete answer');

        // Handle empty response body from controller
        const text = await res.text();
        return text ? JSON.parse(text) : null;
    }

    static async generateQuizWithAI(data) {
        try {
            const response = await fetch(`${API_BASE_URL}/quiz/generate-with-ai`, {
                method: 'POST',
                headers: headers(),
                body: JSON.stringify(data),
            });

            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || error.error || 'Failed to generate quiz with AI');
            }

            const result = await response.json();

            // Transform response to match expected format
            return {
                success: result.success !== false, // Default to true if not specified
                data: result.data || result,
                message: result.message || 'Quiz generated successfully with AI',
            };
        } catch (error) {
            console.error('Error in generateQuizWithAI:', error);
            throw error;
        }
    }
}

export default QuizAPI;