// src/services/teacherAPI.js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

class TeacherAPI {
  static async getTeacherDashboard(teacherId) {
    const response = await fetch(`${API_BASE_URL}/quiz/teacher/me/dashboard`, {
      method: 'GET',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to fetch teacher dashboard');
    
    return result.data;
  }

  static async getTeacherQuizzes(teacherId) {
    const response = await fetch(`${API_BASE_URL}/quiz/teacher/me`, {
      method: 'GET',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to fetch teacher quizzes');
    
    return result.data || []; // Ensure we always return an array
  }
}

export { TeacherAPI };
