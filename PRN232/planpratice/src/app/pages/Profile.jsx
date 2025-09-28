// src/routes/Dashboard.jsx
import { useNavigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';
import styles from './Profile.module.css';

const Profile = () => {
  const navigate = useNavigate();
  const user = AuthAPI.getUser();

  const handleLogout = () => {
    AuthAPI.logout();
    navigate('/login');
  };

  return (
    <div className={styles.container}>
      <nav className={styles.navbar}>
        <div className={styles.navContent}>
          <h1 className={styles.brand}>IELTS Learning Platform</h1>
          <div className={styles.userInfo}>
            <div className={styles.userWelcome}>
              Welcome, <span className={styles.userName}>{user?.username || 'User'}</span>
            </div>
            <button
              onClick={handleLogout}
              className={styles.logoutBtn}
            >
              Logout
            </button>
          </div>
        </div>
      </nav>
      
      <main className={styles.main}>
        <div className={styles.dashboardCard}>
          <h2 className={styles.sectionTitle}>Welcome to your Dashboard</h2>
          <div className={styles.section}>
            <div className={styles.infoList}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>ID:</span> {user?.id}
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Username:</span> {user?.username}
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Email:</span> {user?.email}
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Role:</span> {user?.role}
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Created:</span> {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : 'N/A'}
              </div>
            </div>
          </div>
          
          <div className={styles.section}>
            <h3 className={styles.sectionTitle}>Quick Actions</h3>
            <div className={styles.quickActions}>
              <button className={styles.actionBtn}>
                Create Study Plan
              </button>
              <button className={`${styles.actionBtn} ${styles.blue}`}>
                Browse Lessons
              </button>
              <button className={`${styles.actionBtn} ${styles.purple}`}>
                Take Quiz
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Profile;