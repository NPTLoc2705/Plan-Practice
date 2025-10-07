// src/services/QuizAPI.js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const headers = () => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${localStorage.getItem('token')}`,
});

export class QuizAPI {
    // 🟢 Lấy danh sách quiz của teacher hiện tại
    static async getTeacherQuizzes() {
        const response = await fetch(`${API_BASE_URL}/quiz/teacher/me`, {
            method: 'GET',
            headers: headers(),
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || 'Failed to fetch quizzes');
        }

        return await response.json();
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
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify({
                id, // ✅ keep the same id (some APIs need it)
                title: quizData.title,
                description: quizData.description,
            })
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || 'Failed to update quiz');
        }

        return await response.json();
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

        // ✅ Only parse JSON if response has a body
        let data = null;
        try {
            const text = await response.text();
            data = text ? JSON.parse(text) : null;
        } catch {
            data = null; // Ignore parsing if body is empty
        }

        return data;
    }


    // 🔍 Lấy chi tiết quiz theo id (nếu API backend có)
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
    static async getQuestionsByQuizId(quizId) {
        const res = await fetch(`${API_BASE_URL}/Question/quiz/${quizId}`, {
            method: 'GET',
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to fetch questions');
        return res.json();
    }
    // ========================= QUESTION =========================
    static async getQuestionsByQuizId(quizId) {
        const res = await fetch(`${API_BASE_URL}/Question/quiz/${quizId}`, {
            method: 'GET',
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to fetch questions');
        return res.json();
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
    }

    // ========================= ANSWER =========================
    static async getAnswersByQuestionId(questionId) {
        const res = await fetch(`${API_BASE_URL}/Answer/question/${questionId}`, {
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to fetch answers for question');
        return res.json();
    }

    static async createAnswer(answer) {
        const res = await fetch(`${API_BASE_URL}/Answer`, {
            method: 'POST',
            headers: headers(),
            body: JSON.stringify(answer),
        });
        if (!res.ok) throw new Error('Failed to create answer');
        return res.json();
    }

    static async updateAnswer(id, answer) {
        const res = await fetch(`${API_BASE_URL}/Answer/${id}`, {
            method: 'PUT',
            headers: headers(),
            body: JSON.stringify(answer),
        });
        if (!res.ok) throw new Error('Failed to update answer');
    }

    static async deleteAnswer(id) {
        const res = await fetch(`${API_BASE_URL}/Answer/${id}`, {
            method: 'DELETE',
            headers: headers(),
        });
        if (!res.ok) throw new Error('Failed to delete answer');
    }
}

