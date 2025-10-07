import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { TeacherAPI } from '../components/APIService/TeacherAPI';

const TeacherDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [lessons, setLessons] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  
  // Mock teacher ID - in real app, this would come from user context/auth
  const teacherId = 1;

  useEffect(() => {
    fetchDashboardData();
    fetchLessons();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const data = await TeacherAPI.getTeacherDashboard(teacherId);
      setDashboardData(data);
      setError('');
    } catch (err) {
      console.error('Dashboard error:', err);
      setError(err.message || 'Failed to load dashboard data');
    }
  };

  const fetchLessons = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No authentication token found');
      }

      const response = await fetch('https://localhost:7025/api/Lesson', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('Failed to fetch lessons');
      }

      const responseData = await response.json();
      // Extract lessons from the data property if it exists
      const lessonsData = responseData.data || responseData;
      setLessons(lessonsData);
      setError('');
    } catch (err) {
      console.error('Lessons fetch error:', err);
      setError(err.message || 'Failed to load lessons');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateLesson = () => {
    navigate('/LessonPlanner');
  };

  const handleUpdateLesson = (lessonId) => {
    navigate(`/LessonPlanner/${lessonId}`);
  };

  const handleViewLesson = (lessonId) => {
    navigate(`/LessonPlanner/${lessonId}/view`);
  };

  const handleDeleteLesson = async (lessonId) => {
      try {
        const token = localStorage.getItem('token');
        if (!token) {
          throw new Error('No authentication token found');
        }

        const response = await fetch(`https://localhost:7025/api/Lesson/${lessonId}`, {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });

        if (!response.ok) {
          throw new Error('Failed to delete lesson');
        }

        // Refresh lessons after deletion
        fetchLessons();
      } catch (err) {
        console.error('Delete lesson error:', err);
        setError('Failed to delete lesson');
      }

  };

  if (loading) return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-xl text-gray-600">Loading dashboard...</div>
    </div>
  );
  
  if (error) return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-xl text-red-600">Error: {error}</div>
    </div>
  );
  
  if (!dashboardData) return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-xl text-gray-600">No data available</div>
    </div>
  );

  return (
    <div className="min-h-screen bg-gray-100 p-4 md:p-8">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-800">Teacher Dashboard</h1>
            <p className="mt-2 text-gray-600">
              Track performance, manage quizzes, and plan engaging lessons.
            </p>
          </div>
          <button
            onClick={handleCreateLesson}
            className="mt-4 md:mt-0 bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition duration-200"
          >
            Create New Lesson
          </button>
        </div>

        {/* Statistics Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white p-6 rounded-lg shadow-md">
            <div className="text-3xl font-semibold text-blue-600">{dashboardData.totalTestsCreated}</div>
            <div className="text-gray-600 mt-2">Tests Created</div>
          </div>
          <div className="bg-white p-6 rounded-lg shadow-md">
            <div className="text-3xl font-semibold text-blue-600">{dashboardData.totalTestsTaken}</div>
            <div className="text-gray-600 mt-2">Total Tests Taken</div>
          </div>
          <div className="bg-white p-6 rounded-lg shadow-md">
            <div className="text-3xl font-semibold text-blue-600">{dashboardData.averageScore.toFixed(1)}%</div>
            <div className="text-gray-600 mt-2">Average Score</div>
          </div>
        </div>

        {/* Recent Quizzes */}
        <div className="bg-white p-6 rounded-lg shadow-md mb-8">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">Recent Quizzes</h2>
          {dashboardData.recentQuizzes && dashboardData.recentQuizzes.length > 0 ? (
            <div className="space-y-4">
              {dashboardData.recentQuizzes.map((quiz) => (
                <div
                  key={quiz.id}
                  className="flex flex-col md:flex-row md:items-center md:justify-between p-4 bg-gray-50 rounded-lg"
                >
                  <div>
                    <h3 className="text-lg font-medium text-gray-800">{quiz.title}</h3>
                    <p className="text-sm text-gray-500">
                      Created: {new Date(quiz.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                  <div className="mt-2 md:mt-0 flex space-x-4">
                    <span className="text-sm text-gray-600">Taken: {quiz.timesTaken} times</span>
                    <span className="text-sm text-gray-600">Avg Score: {quiz.averageScore.toFixed(1)}%</span>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-gray-600">No quizzes created yet</p>
          )}
        </div>

        {/* Recent Lessons */}
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">Recent Lessons</h2>
{lessons && lessons.length > 0 ? (
  <div className="max-h-96 overflow-y-auto space-y-4 pr-2 scrollbar-thin scrollbar-thumb-gray-400 scrollbar-track-gray-100">
    {lessons.map((lesson) => (
      <div
        key={lesson.id}
        className="flex flex-col md:flex-row md:items-center md:justify-between p-4 bg-gray-50 rounded-lg"
      >
        <div className="flex-1">
          <h3 className="text-lg font-medium text-gray-800">{lesson.title}</h3>
          <p className="text-sm text-gray-600 mt-1">{lesson.description}</p>
          <p className="text-sm text-gray-500 mt-2">Grade Level: {lesson.gradeLevel}</p>
        </div>
        <div className="mt-4 md:mt-0 md:ml-4 flex space-x-2">
          <button
            onClick={() => handleViewLesson(lesson.id)}
            className="bg-gray-200 text-gray-800 px-3 py-1 rounded-lg hover:bg-gray-300 transition duration-200"
          >
            View
          </button>
          <button
            onClick={() => handleUpdateLesson(lesson.id)}
            className="bg-blue-200 text-blue-800 px-3 py-1 rounded-lg hover:bg-blue-300 transition duration-200"
          >
            Update
          </button>
          <button
            onClick={() => handleDeleteLesson(lesson.id)}
            className="bg-red-200 text-red-800 px-3 py-1 rounded-lg hover:bg-red-300 transition duration-200"
          >
            Delete
          </button>
        </div>
      </div>
    ))}
  </div>
) : (
  <p className="text-gray-600">No lessons created yet</p>
)}

        </div>
      </div>
    </div>
  );
};

export default TeacherDashboard;