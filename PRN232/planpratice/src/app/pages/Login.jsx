// src/routes/Login.jsx
import { useState } from 'react';
import { Eye, EyeOff, Mail, Lock, LogIn, Loader2, AlertCircle, CheckCircle } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';
import GoogleSignIn from '../components/Google/GoogleSignIn';
import styles from './Login.module.css';

const Login = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({ email: '', password: '' });
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setError('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      const result = await AuthAPI.login(formData);
      setSuccess('Login successful! Redirecting...');
      setTimeout(() => navigate('/dashboard'), 1500);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleGoogleSuccess = (result) => {
    setSuccess('Google login successful! Redirecting...');
    setTimeout(() => navigate('/dashboard'), 1500);
  };

  const handleGoogleError = (error) => {
    setError(error);
  };

  return (
    <div className={styles.container}>
      <div className={styles.wrapper}>
        <div className={styles.card}>
          <div className={styles.header}>
            <div className={styles.iconCircle}>
              <LogIn className={styles.icon} />
            </div>
            <h2 className={styles.title}>Welcome Back</h2>
            <p className={styles.subtitle}>Sign in to your IELTS learning platform</p>
          </div>

          {error && (
            <div className={styles.errorMsg}>
              <AlertCircle className={styles.msgIcon} />
              <span>{error}</span>
            </div>
          )}

          {success && (
            <div className={styles.successMsg}>
              <CheckCircle className={styles.msgIcon} />
              <span>{success}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} className={styles.form}>
            <div className={styles.formGroup}>
              <label className={styles.label}>Email</label>
              <div className={styles.inputWrapper}>
                <Mail className={styles.inputIcon} />
                <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  className={styles.input}
                  placeholder="Enter your email"
                  required
                />
              </div>
            </div>

            <div className={styles.formGroup}>
              <label className={styles.label}>Password</label>
              <div className={styles.inputWrapper}>
                <Lock className={styles.inputIcon} />
                <input
                  type={showPassword ? 'text' : 'password'}
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  className={styles.input}
                  placeholder="Enter your password"
                  required
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className={styles.togglePassword}
                >
                  {showPassword ? <EyeOff className={styles.eyeIcon} /> : <Eye className={styles.eyeIcon} />}
                </button>
              </div>
            </div>

            <div className={styles.options}>
              <label className={styles.checkboxLabel}>
                <input type="checkbox" className={styles.checkbox} />
                <span>Remember me</span>
              </label>
              <button className={styles.forgotBtn}>
                Forgot password?
              </button>
            </div>

            <button
              type="submit"
              disabled={loading || !formData.email || !formData.password}
              className={styles.submitBtn}
            >
              {loading ? (
                <>
                  <Loader2 className={styles.loader} />
                  Signing in...
                </>
              ) : (
                'Sign In'
              )}
            </button>
          </form>

          <div className={styles.dividerWrapper}>
            <div className={styles.divider}></div>
            <div className={styles.dividerText}>
              <span>Or continue with</span>
            </div>
          </div>

          <GoogleSignIn onSuccess={handleGoogleSuccess} onError={handleGoogleError} />

          <p className={styles.signupText}>
            Don't have an account?{' '}
            <Link to="/register" className={styles.signupLink}>
              Sign up
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Login;