import React, { useState, useEffect } from 'react';
import { TeacherAPI } from '../components/APIService/TeacherAPI';
import styles from './TeacherDashboard.module.css';

const TeacherDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Mock teacher ID - in real app, this would come from user context/auth
  const teacherId = 1; // You should get this from your auth context

  useEffect(() => {
    fetchDashboardData();
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
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className={styles.loading}>Loading dashboard...</div>;
  if (error) return <div className={styles.error}>Error: {error}</div>;
  if (!dashboardData) return <div className={styles.noData}>No data available</div>;

  return (
    <div className={styles.teacherDashboard}>
      <div className={styles.pageHeader}>
        <h1 className={styles.pageTitle}>Teacher Dashboard</h1>
        <p className={styles.pageSubtitle}>
          Track performance, stay on top of recently published quizzes, and keep your classes engaged.
        </p>
      </div>
      
      {/* Statistics Cards */}
      <div className={styles.statsGrid}>
        <div className={styles.statCard}>
          <div className={styles.statNumber}>{dashboardData.totalTestsCreated}</div>
          <div className={styles.statLabel}>Tests Created</div>
        </div>
        
        <div className={styles.statCard}>
          <div className={styles.statNumber}>{dashboardData.totalTestsTaken}</div>
          <div className={styles.statLabel}>Total Tests Taken</div>
        </div>
        
        <div className={styles.statCard}>
          <div className={styles.statNumber}>{dashboardData.averageScore.toFixed(1)}%</div>
          <div className={styles.statLabel}>Average Score</div>
        </div>
      </div>

      {/* Recent Quizzes */}
      <div className={styles.recentQuizzes}>
        <h2>Recent Quizzes</h2>
        {dashboardData.recentQuizzes && dashboardData.recentQuizzes.length > 0 ? (
          <div className={styles.quizList}>
            {dashboardData.recentQuizzes.map((quiz) => (
              <div key={quiz.id} className={styles.quizItem}>
                <div className={styles.quizInfo}>
                  <h3 className={styles.quizTitle}>{quiz.title}</h3>
                  <p className={styles.quizDate}>
                    Created: {new Date(quiz.createdAt).toLocaleDateString()}
                  </p>
                </div>
                <div className={styles.quizStats}>
                  <span className={styles.quizTaken}>
                    Taken: {quiz.timesTaken} times
                  </span>
                  <span className={styles.quizAvgScore}>
                    Avg Score: {quiz.averageScore.toFixed(1)}%
                  </span>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p className={styles.noQuizzes}>No quizzes created yet</p>
        )}
      </div>
    </div>
  );
};

export default TeacherDashboard;
