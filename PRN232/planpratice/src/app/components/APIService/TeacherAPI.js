// src/services/teacherAPI.js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

class TeacherAPI {
  static async getTeacherDashboard(teacherId) {
    const response = await fetch(`${API_BASE_URL}/quiz/teacher-dashboard/${teacherId}`, {
      method: 'GET',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    if (!response.ok) throw new Error(result.message || 'Failed to fetch teacher dashboard');
    
    const result = await response.json();
    return result;
  }

  static async getTeacherQuizzes(teacherId) {
    const response = await fetch(`${API_BASE_URL}/quiz/teacher/${teacherId}`, {
      method: 'GET',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    const result = await response.json();
    if (!response.ok) throw new Error(result.message || 'Failed to fetch teacher quizzes');
    return result;
  }
}

export { TeacherAPI };
