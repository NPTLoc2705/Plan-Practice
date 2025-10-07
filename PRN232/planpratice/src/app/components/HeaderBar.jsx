import { Link } from 'react-router-dom';
import styles from './HeaderBar.module.css';

const HeaderBar = () => (
  <header className={styles.headerBar}>
    <div className={styles.logoSection}>
      <h1 className={styles.logoText}>PlanPractice</h1>
      <span className={styles.logoSubtitle}>Learn • Teach • Excel</span>
    </div>
    <nav className={styles.navbar}>
      <Link to="/landing" className={styles.navLink}>Home</Link>
      <Link to="/quizmanagement" className={styles.navLink}>Quiz Management</Link>
      <Link to="/teacher" className={styles.navLink}>Teacher Dashboard</Link>
      <Link to="/profile" className={styles.navLink}>Profile</Link>
    </nav>
  </header>
);

export default HeaderBar;