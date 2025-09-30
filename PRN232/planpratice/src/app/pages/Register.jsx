// src/routes/Register.jsx
import { useState, useEffect } from 'react';
import { Eye, EyeOff, Mail, Lock, User, UserPlus, Loader2, AlertCircle, CheckCircle, ArrowLeft } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';
import GoogleSignIn from '../components/Google/GoogleSignIn';
import styles from './Register.module.css';

const Register = () => {
  const navigate = useNavigate();
  const [step, setStep] = useState(1);
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    otp: ''
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [resendTimer, setResendTimer] = useState(0);

  useEffect(() => {
    if (resendTimer > 0) {
      const timer = setTimeout(() => setResendTimer(resendTimer - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendTimer]);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setError('');
  };

  const handleRegister = async () => {
    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    setLoading(true);
    setError('');
    
    try {
      const result = await AuthAPI.register({
        username: formData.username,
        email: formData.email,
        password: formData.password
      });
      setSuccess('OTP sent to your email. Please check your inbox.');
      setStep(2);
      setResendTimer(60);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOTP = async () => {
    setLoading(true);
    setError('');
    
    try {
      const result = await AuthAPI.verifyRegistration({
        email: formData.email,
        username: formData.username,
        password: formData.password,
        otp: formData.otp
      });
      setSuccess('Registration successful! Redirecting to login...');
      setTimeout(() => navigate('/login'), 2000);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleResendOTP = async () => {
    if (resendTimer > 0) return;
    
    setLoading(true);
    setError('');
    
    try {
      const result = await AuthAPI.resendOTP(formData.email);
      setSuccess('New OTP sent to your email');
      setResendTimer(60);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleGoogleSuccess = (result) => {
    setSuccess('Google registration successful! Redirecting...');
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
              <UserPlus className={styles.icon} />
            </div>
            <h2 className={styles.title}>Create Account</h2>
            <p className={styles.subtitle}>
              {step === 1 ? 'Join our IELTS learning platform' : 'Verify your email address'}
            </p>
          </div>

          <div className={styles.progress}>
            <div className={`${styles.progressStep} ${step >= 1 ? styles.progressActive : styles.progressInactive}`}>1</div>
            <div className={`${styles.progressBar} ${step >= 2 ? styles.progressBarActive : ''}`}></div>
            <div className={`${styles.progressStep} ${step >= 2 ? styles.progressActive : styles.progressInactive}`}>2</div>
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

          {step === 1 ? (
            <>
              <div className={styles.formGroup}>
                <label className={styles.label}>Username</label>
                <div className={styles.inputWrapper}>
                  <User className={styles.inputIcon} />
                  <input
                    type="text"
                    name="username"
                    value={formData.username}
                    onChange={handleChange}
                    className={styles.input}
                    placeholder="Choose a username"
                    required
                  />
                </div>
              </div>

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
                    placeholder="Create a password"
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

              <div className={styles.formGroup}>
                <label className={styles.label}>Confirm Password</label>
                <div className={styles.inputWrapper}>
                  <Lock className={styles.inputIcon} />
                  <input
                    type={showConfirmPassword ? 'text' : 'password'}
                    name="confirmPassword"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    className={styles.input}
                    placeholder="Confirm your password"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                    className={styles.togglePassword}
                  >
                    {showConfirmPassword ? <EyeOff className={styles.eyeIcon} /> : <Eye className={styles.eyeIcon} />}
                  </button>
                </div>
              </div>

              <button
                onClick={handleRegister}
                disabled={loading || !formData.username || !formData.email || !formData.password || !formData.confirmPassword}
                className={styles.submitBtn}
              >
                {loading ? (
                  <>
                    <Loader2 className={styles.loader} />
                    Creating account...
                  </>
                ) : (
                  'Continue'
                )}
              </button>

              <div className={styles.dividerWrapper}>
                <div className={styles.divider}></div>
                <div className={styles.dividerText}>
                  <span>Or sign up with</span>
                </div>
              </div>

              <GoogleSignIn onSuccess={handleGoogleSuccess} onError={handleGoogleError} />
            </>
          ) : (
            <div>
              <div className={styles.otpBox}>
                <p className={styles.subtitle} style={{ marginBottom: 8 }}>
                  We've sent a 6-digit verification code to:
                </p>
                <p className={styles.title} style={{ fontSize: '1rem', color: '#a21caf' }}>{formData.email}</p>
              </div>

              <div className={styles.formGroup}>
                <label className={styles.label}>Verification Code</label>
                <input
                  type="text"
                  name="otp"
                  value={formData.otp}
                  onChange={handleChange}
                  maxLength="6"
                  className={styles.otpInput}
                  placeholder="000000"
                  required
                />
              </div>

              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', fontSize: '0.95rem', marginBottom: 12 }}>
                <button
                  type="button"
                  onClick={() => setStep(1)}
                  className={styles.backBtn}
                >
                  <ArrowLeft style={{ width: 16, height: 16 }} />
                  Back
                </button>
                <button
                  type="button"
                  onClick={handleResendOTP}
                  disabled={resendTimer > 0 || loading}
                  className={styles.resendBtn}
                  style={resendTimer > 0 || loading ? { color: '#a1a1aa', cursor: 'not-allowed' } : {}}
                >
                  {resendTimer > 0 ? `Resend in ${resendTimer}s` : 'Resend code'}
                </button>
              </div>

              <button
                onClick={handleVerifyOTP}
                disabled={loading || formData.otp.length !== 6}
                className={styles.submitBtn}
              >
                {loading ? (
                  <>
                    <Loader2 className={styles.loader} />
                    Verifying...
                  </>
                ) : (
                  'Verify & Complete'
                )}
              </button>
            </div>
          )}

          <p className={styles.loginText}>
            Already have an account?{' '}
            <Link to="/login" className={styles.loginLink}>
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;