import React from 'react';
import { Link } from 'react-router-dom';
import styles from './Landing.module.css';

const Landing = () => {
  return (
    <div className={styles.landingContainer}>
      {/* Header */}
      <header className={styles.landingHeader}>
        <div className={styles.container}>
          <div className={styles.logoSection}>
            <h1 className={styles.logoText}>PlanPractice</h1>
            <span className={styles.logoSubtitle}>Learn • Teach • Excel</span>
          </div>
          <nav className={styles.navigation}>
            <Link to="/login" className={styles.navLink}>Login</Link>
            <Link to="/register" className={`${styles.navLink} ${styles.navLinkPrimary}`}>Sign Up</Link>
          </nav>
        </div>
      </header>

      {/* Hero Section */}
      <section className={styles.heroSection}>
        <div className={styles.container}>
          <div className={styles.heroContent}>
            <h1 className={styles.heroTitle}>
              Revolutionize Your Learning Experience
            </h1>
            <p className={styles.heroDescription}>
              Empower teachers to create engaging lessons and interactive quizzes. 
              Enable students to learn, practice, and excel in their studies.
            </p>
            <div className={styles.heroButtons}>
              <Link to="/register" className={`${styles.btn} ${styles.btnPrimary}`}>Get Started Free</Link>
              <Link to="#features" className={`${styles.btn} ${styles.btnSecondary}`}>Learn More</Link>
            </div>
          </div>
          <div className={styles.heroImage}>
            <div className={styles.heroIllustration}>
              <div className={styles.illustrationCard}>
                <div className={styles.cardIcon}>📚</div>
                <div className={styles.cardText}>Interactive Learning</div>
              </div>
              <div className={styles.illustrationCard}>
                <div className={styles.cardIcon}>🎓</div>
                <div className={styles.cardText}>Expert Teachers</div>
              </div>
              <div className={styles.illustrationCard}>
                <div className={styles.cardIcon}>📊</div>
                <div className={styles.cardText}>Track Progress</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className={styles.featuresSection}>
        <div className={styles.container}>
          <div className={styles.sectionHeader}>
            <h2 className={styles.sectionTitle}>Why Choose PlanPractice?</h2>
            <p className={styles.sectionDescription}>
              Our platform bridges the gap between teaching and learning with powerful tools
            </p>
          </div>
          
          <div className={styles.featuresGrid}>
            {/* Teacher Features */}
            <div className={styles.featureCategory}>
              <div className={styles.categoryHeader}>
                <div className={styles.categoryIcon}>👨‍🏫</div>
                <h3 className={styles.categoryTitle}>For Teachers</h3>
              </div>
              
              <div className={styles.featureCard}>
                <div className={styles.featureIcon}>📝</div>
                <div>
                  <h4 className={styles.featureTitle}>Create Lessons</h4>
                  <p className={styles.featureDescription}>
                    Design comprehensive lessons with rich content, multimedia, and interactive elements
                  </p>
                </div>
              </div>
              
              <div className={styles.featureCard}>
                <div className={styles.featureIcon}>❓</div>
                <div>
                  <h4 className={styles.featureTitle}>Build Quizzes</h4>
                  <p className={styles.featureDescription}>
                    Create engaging quizzes with multiple question types and instant feedback
                  </p>
                </div>
              </div>
              
              <div className={styles.featureCard}>
                <div className={styles.featureIcon}>📈</div>
                <div>
                  <h4 className={styles.featureTitle}>Track Performance</h4>
                  <p className={styles.featureDescription}>
                    Monitor student progress and identify areas that need attention
                  </p>
                </div>
              </div>
            </div>

            {/* Student Features */}
            <div className={styles.featureCategory}>
              <div className={styles.categoryHeader}>
                <div className={styles.categoryIcon}>👨‍🎓</div>
                <h3 className={styles.categoryTitle}>For Students</h3>
              </div>
              
              <div className={styles.featureCard}>
                <div className={styles.featureIcon}>📖</div>
                <div>
                  <h4 className={styles.featureTitle}>Interactive Learning</h4>
                  <p className={styles.featureDescription}>
                    Access engaging lessons designed to make learning enjoyable and effective
                  </p>
                </div>
              </div>
              
              <div className={styles.featureCard}>
                <div className={styles.featureIcon}>🎯</div>
                <div>
                  <h4 className={styles.featureTitle}>Take Quizzes</h4>
                  <p className={styles.featureDescription}>
                    Test your knowledge with interactive quizzes and receive instant feedback
                  </p>
                </div>
              </div>
              
              <div className={styles.featureCard}>
                <div className={styles.featureIcon}>🏆</div>
                <div>
                  <h4 className={styles.featureTitle}>Achieve Goals</h4>
                  <p className={styles.featureDescription}>
                    Set learning goals and track your progress towards academic success
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* How It Works Section */}
      <section className={styles.howItWorksSection}>
        <div className={styles.container}>
          <div className={styles.sectionHeader}>
            <h2 className={styles.sectionTitle}>How It Works</h2>
            <p className={styles.sectionDescription}>Get started in three simple steps</p>
          </div>
          
          <div className={styles.stepsGrid}>
            <div className={styles.stepCard}>
              <div className={styles.stepNumber}>1</div>
              <h3 className={styles.stepTitle}>Sign Up</h3>
              <p className={styles.stepDescription}>
                Create your account and choose your role - Teacher or Student
              </p>
            </div>
            
            <div className={styles.stepCard}>
              <div className={styles.stepNumber}>2</div>
              <h3 className={styles.stepTitle}>Create or Join</h3>
              <p className={styles.stepDescription}>
                Teachers create lessons and quizzes. Students join classes and start learning
              </p>
            </div>
            
            <div className={styles.stepCard}>
              <div className={styles.stepNumber}>3</div>
              <h3 className={styles.stepTitle}>Learn & Grow</h3>
              <p className={styles.stepDescription}>
                Engage with content, track progress, and achieve your learning goals
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className={styles.ctaSection}>
        <div className={styles.container}>
          <div className={styles.ctaContent}>
            <h2 className={styles.ctaTitle}>Ready to Transform Your Learning Journey?</h2>
            <p className={styles.ctaDescription}>
              Join thousands of teachers and students already using PlanPractice
            </p>
            <div className={styles.ctaButtons}>
              <Link to="/register" className={`${styles.btn} ${styles.btnPrimary} ${styles.btnLarge}`}>
                Start Learning Today
              </Link>
              <Link to="/login" className={`${styles.btn} ${styles.btnOutline} ${styles.btnLarge}`}>
                Already have an account?
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className={styles.landingFooter}>
        <div className={styles.container}>
          <div className={styles.footerContent}>
            <div className={styles.footerSection}>
              <h3 className={styles.footerTitle}>PlanPractice</h3>
              <p className={styles.footerDescription}>
                Empowering education through innovative technology
              </p>
            </div>
            
            <div className={styles.footerSection}>
              <h4 className={styles.footerHeading}>Platform</h4>
              <ul className={styles.footerLinks}>
                <li><Link to="/features">Features</Link></li>
                <li><Link to="/pricing">Pricing</Link></li>
                <li><Link to="/support">Support</Link></li>
              </ul>
            </div>
            
            <div className={styles.footerSection}>
              <h4 className={styles.footerHeading}>Account</h4>
              <ul className={styles.footerLinks}>
                <li><Link to="/login">Login</Link></li>
                <li><Link to="/register">Sign Up</Link></li>
                <li><Link to="/profile">Profile</Link></li>
              </ul>
            </div>
          </div>
          
          <div className={styles.footerBottom}>
            <p>&copy; 2025 PlanPractice. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default Landing;
