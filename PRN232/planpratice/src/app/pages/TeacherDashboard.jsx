import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { TeacherAPI } from '../components/APIService/TeacherAPI';

const TeacherDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const data = await TeacherAPI.getTeacherDashboard();
      setDashboardData(data);
      setError('');
    } catch (err) {
      console.error('Dashboard error:', err);
      setError(err.message || 'Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateLesson = () => {
    navigate('LessonPlanner');
  };

  const handleUpdateLesson = (lessonId) => {
    navigate(`LessonPlanner/${lessonId}`);
  };

  const handleViewLesson = (lessonId) => {
    navigate(`LessonPlanner/${lessonId}/view`);
  };

  const handleCreateQuiz = () => {
    navigate('/teacher/quiz/create');
  };

  const handleViewQuiz = (quizId) => {
    navigate(`/teacher/quiz/${quizId}`);
  };

  if (loading) return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      <div className="text-center">
        <div className="animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-blue-600 mx-auto"></div>
        <p className="mt-4 text-xl text-gray-700">Loading dashboard...</p>
      </div>
    </div>
  );

  if (error) return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-red-50 to-pink-100">
      <div className="bg-white p-8 rounded-lg shadow-lg max-w-md">
        <div className="text-red-600 text-6xl mb-4 text-center">⚠️</div>
        <h2 className="text-2xl font-bold text-gray-800 mb-2 text-center">Error</h2>
        <p className="text-gray-600 text-center">{error}</p>
        <button
          onClick={fetchDashboardData}
          className="mt-4 w-full bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition duration-200"
        >
          Retry
        </button>
      </div>
    </div>
  );

  if (!dashboardData) return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-xl text-gray-600">No data available</div>
    </div>
  );

  // Calculate total performance percentage
  const totalResults = dashboardData.quizPerformance.excellentCount +
    dashboardData.quizPerformance.goodCount +
    dashboardData.quizPerformance.averageCount +
    dashboardData.quizPerformance.belowAverageCount;

  const getPerformancePercentage = (count) => {
    return totalResults > 0 ? ((count / totalResults) * 100).toFixed(1) : 0;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 p-4 md:p-8">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between">
            <div>
              <h1 className="text-4xl font-bold text-gray-800 mb-2">Teacher Dashboard</h1>
              <p className="text-gray-600">
                Welcome back! Here's your teaching overview.
              </p>
            </div>
            <div className="mt-4 md:mt-0 flex gap-3">
              <button
                onClick={handleCreateQuiz}
                className="bg-purple-600 text-white px-6 py-3 rounded-lg hover:bg-purple-700 transition duration-200 shadow-md hover:shadow-lg font-semibold"
              >
                + Create Quiz
              </button>
              <button
                onClick={handleCreateLesson}
                className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition duration-200 shadow-md hover:shadow-lg font-semibold"
              >
                + Create Lesson
              </button>
            </div>
          </div>
        </div>

        {/* Main Statistics Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          {/* Total Lessons */}
          <div className="bg-gradient-to-br from-blue-500 to-blue-600 p-6 rounded-xl shadow-lg text-white transform hover:scale-105 transition duration-300">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-blue-100 text-sm font-medium mb-1">Total Lessons</p>
                <p className="text-4xl font-bold">{dashboardData.totalLessonsCreated}</p>
              </div>
              <div className="text-5xl opacity-20">📚</div>
            </div>
            <div className="mt-4 text-sm text-blue-100">
              Lesson plans created
            </div>
          </div>

          {/* Total Quizzes */}
          <div className="bg-gradient-to-br from-purple-500 to-purple-600 p-6 rounded-xl shadow-lg text-white transform hover:scale-105 transition duration-300">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-purple-100 text-sm font-medium mb-1">Total Quizzes</p>
                <p className="text-4xl font-bold">{dashboardData.totalQuizzesCreated}</p>
              </div>
              <div className="text-5xl opacity-20">📝</div>
            </div>
            <div className="mt-4 text-sm text-purple-100">
              Quizzes created
            </div>
          </div>

          {/* Students Engaged */}
          <div className="bg-gradient-to-br from-green-500 to-green-600 p-6 rounded-xl shadow-lg text-white transform hover:scale-105 transition duration-300">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-green-100 text-sm font-medium mb-1">Students Engaged</p>
                <p className="text-4xl font-bold">{dashboardData.totalStudentsEngaged}</p>
              </div>
              <div className="text-5xl opacity-20">👥</div>
            </div>
            <div className="mt-4 text-sm text-green-100">
              Unique students reached
            </div>
          </div>

          {/* Average Score */}
          <div className="bg-gradient-to-br from-orange-500 to-orange-600 p-6 rounded-xl shadow-lg text-white transform hover:scale-105 transition duration-300">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-orange-100 text-sm font-medium mb-1">Average Score</p>
                <p className="text-4xl font-bold">{dashboardData.averageQuizScore.toFixed(1)}%</p>
              </div>
              <div className="text-5xl opacity-20">⭐</div>
            </div>
            <div className="mt-4 text-sm text-orange-100">
              Overall quiz performance
            </div>
          </div>
        </div>

        {/* Recent Lessons Section */}
        <div className="bg-white p-6 rounded-xl shadow-md hover:shadow-lg transition duration-300">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold text-gray-800">Recent Lessons</h2>
            <button
              onClick={handleCreateLesson}
              className="text-blue-600 hover:text-blue-700 font-semibold text-sm"
            >
              View All →
            </button>
          </div>
          {dashboardData.recentLessons && dashboardData.recentLessons.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {dashboardData.recentLessons.map((lesson) => (
                <div
                  key={lesson.id}
                  className="p-4 bg-gradient-to-br from-blue-50 to-indigo-50 rounded-lg hover:shadow-md transition duration-200 cursor-pointer border border-blue-100"
                  onClick={() => handleViewLesson(lesson.id)}
                >
                  <div className="flex items-start justify-between mb-2">
                    <h3 className="text-lg font-semibold text-gray-800 flex-1 pr-2">{lesson.title}</h3>
                    <span className="text-2xl">📖</span>
                  </div>
                  <div className="mb-3">
                    <span className="inline-block bg-blue-200 text-blue-800 text-xs font-semibold px-3 py-1 rounded-full">
                      {lesson.gradeLevel}
                    </span>
                  </div>
                  <p className="text-xs text-gray-500">
                    Created: {new Date(lesson.createdAt).toLocaleDateString('en-US', {
                      month: 'short',
                      day: 'numeric',
                      year: 'numeric'
                    })}
                  </p>
                  {lesson.updatedAt && (
                    <p className="text-xs text-gray-500">
                      Updated: {new Date(lesson.updatedAt).toLocaleDateString('en-US', {
                        month: 'short',
                        day: 'numeric',
                        year: 'numeric'
                      })}
                    </p>
                  )}
                  <div className="mt-3 flex gap-2">
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        handleViewLesson(lesson.id);
                      }}
                      className="flex-1 bg-blue-100 text-blue-700 px-3 py-1 rounded text-sm hover:bg-blue-200 transition duration-200"
                    >
                      View
                    </button>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        handleUpdateLesson(lesson.id);
                      }}
                      className="flex-1 bg-indigo-100 text-indigo-700 px-3 py-1 rounded text-sm hover:bg-indigo-200 transition duration-200"
                    >
                      Edit
                    </button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12">
              <div className="text-6xl mb-4">📚</div>
              <p className="text-gray-600 mb-4">No lessons created yet</p>
              <button
                onClick={handleCreateLesson}
                className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition duration-200"
              >
                Create Your First Lesson
              </button>
            </div>
          )}
        </div>

        {/* Secondary Statistics */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
          {/* Quiz Attempts */}
          <div className="bg-white p-6 rounded-xl shadow-md hover:shadow-lg transition duration-300">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold text-gray-800">Quiz Activity</h3>
              <span className="text-3xl">📊</span>
            </div>
            <div className="text-3xl font-bold text-indigo-600 mb-2">
              {dashboardData.totalQuizzesTaken}
            </div>
            <p className="text-gray-600 text-sm">Total quiz attempts by students</p>
          </div>

          {/* Performance Breakdown */}
          <div className="bg-white p-6 rounded-xl shadow-md hover:shadow-lg transition duration-300">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold text-gray-800">Performance Distribution</h3>
              <span className="text-3xl">📈</span>
            </div>
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <div className="flex items-center">
                  <div className="w-3 h-3 bg-green-500 rounded-full mr-2"></div>
                  <span className="text-sm text-gray-700">Excellent (90-100%)</span>
                </div>
                <span className="font-semibold text-gray-800">
                  {dashboardData.quizPerformance.excellentCount} ({getPerformancePercentage(dashboardData.quizPerformance.excellentCount)}%)
                </span>
              </div>
              <div className="flex items-center justify-between">
                <div className="flex items-center">
                  <div className="w-3 h-3 bg-blue-500 rounded-full mr-2"></div>
                  <span className="text-sm text-gray-700">Good (70-89%)</span>
                </div>
                <span className="font-semibold text-gray-800">
                  {dashboardData.quizPerformance.goodCount} ({getPerformancePercentage(dashboardData.quizPerformance.goodCount)}%)
                </span>
              </div>
              <div className="flex items-center justify-between">
                <div className="flex items-center">
                  <div className="w-3 h-3 bg-yellow-500 rounded-full mr-2"></div>
                  <span className="text-sm text-gray-700">Average (50-69%)</span>
                </div>
                <span className="font-semibold text-gray-800">
                  {dashboardData.quizPerformance.averageCount} ({getPerformancePercentage(dashboardData.quizPerformance.averageCount)}%)
                </span>
              </div>
              <div className="flex items-center justify-between">
                <div className="flex items-center">
                  <div className="w-3 h-3 bg-red-500 rounded-full mr-2"></div>
                  <span className="text-sm text-gray-700">Below Average (&lt;50%)</span>
                </div>
                <span className="font-semibold text-gray-800">
                  {dashboardData.quizPerformance.belowAverageCount} ({getPerformancePercentage(dashboardData.quizPerformance.belowAverageCount)}%)
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Recent Quizzes Section */}
        <div className="bg-white p-6 rounded-xl shadow-md hover:shadow-lg transition duration-300 mb-8">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold text-gray-800">Recent Quizzes</h2>
            <button
              onClick={handleCreateQuiz}
              className="text-purple-600 hover:text-purple-700 font-semibold text-sm"
            >
              View All →
            </button>
          </div>
          {dashboardData.recentQuizzes && dashboardData.recentQuizzes.length > 0 ? (
            <div className="space-y-4">
              {dashboardData.recentQuizzes.map((quiz) => (
                <div
                  key={quiz.id}
                  className="flex flex-col md:flex-row md:items-center md:justify-between p-4 bg-gradient-to-r from-purple-50 to-indigo-50 rounded-lg hover:shadow-md transition duration-200 cursor-pointer"
                  onClick={() => handleViewQuiz(quiz.id)}
                >
                  <div className="flex-1">
                    <h3 className="text-lg font-semibold text-gray-800 mb-1">{quiz.title}</h3>
                    <p className="text-sm text-gray-500">
                      Created: {new Date(quiz.createdAt).toLocaleDateString('en-US', {
                        year: 'numeric',
                        month: 'short',
                        day: 'numeric'
                      })}
                    </p>
                  </div>
                  <div className="mt-3 md:mt-0 flex gap-6">
                    <div className="text-center">
                      <p className="text-2xl font-bold text-purple-600">{quiz.timesTaken}</p>
                      <p className="text-xs text-gray-500">Attempts</p>
                    </div>
                    <div className="text-center">
                      <p className="text-2xl font-bold text-indigo-600">{quiz.averageScore.toFixed(1)}%</p>
                      <p className="text-xs text-gray-500">Avg Score</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12">
              <div className="text-6xl mb-4">📝</div>
              <p className="text-gray-600 mb-4">No quizzes created yet</p>
              <button
                onClick={handleCreateQuiz}
                className="bg-purple-600 text-white px-6 py-2 rounded-lg hover:bg-purple-700 transition duration-200"
              >
                Create Your First Quiz
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TeacherDashboard;