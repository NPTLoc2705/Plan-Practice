import { useState, useEffect } from 'react';
import { Eye, EyeOff, Mail, Lock, User, UserPlus, Loader2, AlertCircle, CheckCircle, ArrowLeft, GraduationCap, BookOpen } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';
import GoogleSignIn from '../components/Google/GoogleSignIn';

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
  const [selectedRole, setSelectedRole] = useState('');
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
      await AuthAPI.register({
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
      await AuthAPI.verifyRegistration({
        email: formData.email,
        username: formData.username,
        password: formData.password,
        otp: formData.otp
      });
      setSuccess('Registration verified successfully!');
      setStep(3); // Move to role selection step
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleRoleSelection = async () => {
    if (!selectedRole) {
      setError('Please select a role');
      return;
    }

    setLoading(true);
    setError('');

    try {
      if (selectedRole === 'teacher') {
        await AuthAPI.updateTeacherRole(formData.email);
        setSuccess('Role updated to Teacher successfully! Redirecting to login...');
      } else {
        setSuccess('Registration completed successfully! Redirecting to login...');
      }
      
      setTimeout(() => {
        navigate('/login', {
          state: {
            message: `Registration successful! You can now log in as a ${selectedRole}.`,
            email: formData.email
          }
        });
      }, 2000);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSkipRoleSelection = () => {
    setSuccess('Registration completed successfully! Redirecting to login...');
    setTimeout(() => {
      navigate('/login', {
        state: {
          message: 'Registration successful! You can now log in.',
          email: formData.email
        }
      });
    }, 1500);
  };

  const handleResendOTP = async () => {
    if (resendTimer > 0) return;
    
    setLoading(true);
    setError('');
    
    try {
      await AuthAPI.resendOTP(formData.email);
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
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-purple-50 via-blue-50 to-white px-4">
      <div className="w-full max-w-lg mx-auto">
        <div className="bg-white rounded-2xl shadow-xl p-8 flex flex-col gap-6">
          <div className="flex flex-col items-center gap-2">
            <div className="inline-flex items-center justify-center w-14 h-14 bg-purple-100 rounded-full mb-2">
              <UserPlus className="w-8 h-8 text-purple-600" />
            </div>
            <h2 className="text-2xl font-bold text-gray-900">
              {step === 3 ? 'Choose Your Role' : 'Create Account'}
            </h2>
            <p className="text-gray-500 text-sm">
              {step === 1 && 'Join our IELTS learning platform'}
              {step === 2 && 'Verify your email address'}
              {step === 3 && 'Select how you\'d like to use our platform'}
            </p>
          </div>

          {/* Progress Bar */}
          <div className="flex items-center justify-center gap-2 mb-2">
            <div className={`flex items-center justify-center w-8 h-8 rounded-full font-bold text-white ${step >= 1 ? 'bg-purple-600' : 'bg-gray-300'}`}>1</div>
            <div className={`w-12 h-1 rounded-full ${step >= 2 ? 'bg-purple-600' : 'bg-gray-200'}`}></div>
            <div className={`flex items-center justify-center w-8 h-8 rounded-full font-bold text-white ${step >= 2 ? 'bg-purple-600' : 'bg-gray-300'}`}>2</div>
            <div className={`w-12 h-1 rounded-full ${step >= 3 ? 'bg-purple-600' : 'bg-gray-200'}`}></div>
            <div className={`flex items-center justify-center w-8 h-8 rounded-full font-bold text-white ${step >= 3 ? 'bg-purple-600' : 'bg-gray-300'}`}>3</div>
          </div>

          {error && (
            <div className="flex items-center gap-2 bg-red-50 border border-red-200 text-red-700 rounded-lg px-4 py-2 text-sm">
              <AlertCircle className="w-5 h-5" />
              <span>{error}</span>
            </div>
          )}

          {success && (
            <div className="flex items-center gap-2 bg-green-50 border border-green-200 text-green-700 rounded-lg px-4 py-2 text-sm">
              <CheckCircle className="w-5 h-5" />
              <span>{success}</span>
            </div>
          )}

          {step === 1 ? (
            <>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Username</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <input
                    type="text"
                    name="username"
                    value={formData.username}
                    onChange={handleChange}
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:outline-none bg-gray-50 text-gray-900 transition"
                    placeholder="Choose a username"
                    required
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:outline-none bg-gray-50 text-gray-900 transition"
                    placeholder="Enter your email"
                    required
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <input
                    type={showPassword ? 'text' : 'password'}
                    name="password"
                    value={formData.password}
                    onChange={handleChange}
                    className="w-full pl-10 pr-10 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:outline-none bg-gray-50 text-gray-900 transition"
                    placeholder="Create a password"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-purple-600"
                    tabIndex={-1}
                  >
                    {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                  </button>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Confirm Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <input
                    type={showConfirmPassword ? 'text' : 'password'}
                    name="confirmPassword"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    className="w-full pl-10 pr-10 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:outline-none bg-gray-50 text-gray-900 transition"
                    placeholder="Confirm your password"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                    className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-purple-600"
                    tabIndex={-1}
                  >
                    {showConfirmPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                  </button>
                </div>
              </div>

              <button
                onClick={handleRegister}
                disabled={loading || !formData.username || !formData.email || !formData.password || !formData.confirmPassword}
                className="w-full bg-purple-600 text-white font-semibold py-2 rounded-lg shadow hover:bg-purple-700 transition disabled:opacity-50 flex items-center justify-center gap-2 mt-2"
              >
                {loading ? (
                  <>
                    <Loader2 className="w-5 h-5 animate-spin" />
                    Creating account...
                  </>
                ) : (
                  'Continue'
                )}
              </button>

              <div className="relative my-4">
                <div className="absolute inset-0 flex items-center">
                  <div className="w-full border-t border-gray-200"></div>
                </div>
                <div className="relative flex justify-center">
                  <span className="bg-white px-3 text-gray-500 text-sm">Or sign up with</span>
                </div>
              </div>

              <GoogleSignIn onSuccess={handleGoogleSuccess} onError={handleGoogleError} />
            </>
          ) : step === 2 ? (
            <div>
              <div className="text-center p-4 bg-purple-50 rounded-lg mb-4">
                <p className="text-gray-700 mb-2">
                  We've sent a 6-digit verification code to:
                </p>
                <p className="font-bold text-purple-600">{formData.email}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Verification Code</label>
                <input
                  type="text"
                  name="otp"
                  value={formData.otp}
                  onChange={handleChange}
                  maxLength="6"
                  className="w-full py-3 text-2xl text-center tracking-widest border border-gray-300 rounded-lg bg-gray-50 focus:ring-2 focus:ring-purple-500 focus:outline-none transition"
                  placeholder="000000"
                  required
                />
              </div>

              <div className="flex items-center justify-between text-sm mt-4 mb-2">
                <button
                  type="button"
                  onClick={() => setStep(1)}
                  className="flex items-center gap-1 text-gray-600 hover:text-gray-900 transition"
                >
                  <ArrowLeft className="w-4 h-4" />
                  Back
                </button>
                <button
                  type="button"
                  onClick={handleResendOTP}
                  disabled={resendTimer > 0 || loading}
                  className={`font-medium transition ${
                    resendTimer > 0 || loading
                      ? 'text-gray-400 cursor-not-allowed'
                      : 'text-purple-600 hover:underline'
                  }`}
                >
                  {resendTimer > 0 ? `Resend in ${resendTimer}s` : 'Resend code'}
                </button>
              </div>

              <button
                onClick={handleVerifyOTP}
                disabled={loading || formData.otp.length !== 6}
                className="w-full bg-purple-600 text-white font-semibold py-2 rounded-lg shadow hover:bg-purple-700 transition disabled:opacity-50 flex items-center justify-center gap-2 mt-2"
              >
                {loading ? (
                  <>
                    <Loader2 className="w-5 h-5 animate-spin" />
                    Verifying...
                  </>
                ) : (
                  'Verify & Continue'
                )}
              </button>
            </div>
          ) : (
            // Step 3: Role Selection
            <div className="space-y-6">
              <div className="space-y-4">
                {/* Teacher Option */}
                <div 
                  className={`relative border-2 rounded-xl p-4 cursor-pointer transition-all duration-200 ${
                    selectedRole === 'teacher' 
                      ? 'border-purple-500 bg-purple-50' 
                      : 'border-gray-300 hover:border-gray-400'
                  }`}
                  onClick={() => setSelectedRole('teacher')}
                >
                  <div className="flex items-start">
                    <div className="flex items-center h-5">
                      <input
                        id="teacher"
                        name="role"
                        type="radio"
                        value="teacher"
                        checked={selectedRole === 'teacher'}
                        onChange={(e) => setSelectedRole(e.target.value)}
                        className="focus:ring-purple-500 h-4 w-4 text-purple-600 border-gray-300"
                      />
                    </div>
                    <div className="ml-3">
                      <label htmlFor="teacher" className="font-medium text-gray-900 cursor-pointer">
                        Teacher
                      </label>
                      <p className="text-sm text-gray-500">
                        Create and manage quizzes, track student progress
                      </p>
                    </div>
                  </div>
                  <div className="absolute top-4 right-4">
                    <GraduationCap className="w-6 h-6 text-purple-500" />
                  </div>
                </div>

                {/* Student Option */}
                <div 
                  className={`relative border-2 rounded-xl p-4 cursor-pointer transition-all duration-200 ${
                    selectedRole === 'student' 
                      ? 'border-purple-500 bg-purple-50' 
                      : 'border-gray-300 hover:border-gray-400'
                  }`}
                  onClick={() => setSelectedRole('student')}
                >
                  <div className="flex items-start">
                    <div className="flex items-center h-5">
                      <input
                        id="student"
                        name="role"
                        type="radio"
                        value="student"
                        checked={selectedRole === 'student'}
                        onChange={(e) => setSelectedRole(e.target.value)}
                        className="focus:ring-purple-500 h-4 w-4 text-purple-600 border-gray-300"
                      />
                    </div>
                    <div className="ml-3">
                      <label htmlFor="student" className="font-medium text-gray-900 cursor-pointer">
                        Student
                      </label>
                      <p className="text-sm text-gray-500">
                        Take quizzes and track your learning progress
                      </p>
                    </div>
                  </div>
                  <div className="absolute top-4 right-4">
                    <BookOpen className="w-6 h-6 text-blue-500" />
                  </div>
                </div>
              </div>

              <div className="flex space-x-4">
                <button
                  onClick={handleRoleSelection}
                  disabled={!selectedRole || loading}
                  className={`group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 ${
                    !selectedRole || loading
                      ? 'bg-gray-400 cursor-not-allowed'
                      : 'bg-purple-600 hover:bg-purple-700'
                  }`}
                >
                  {loading ? (
                    <>
                      <Loader2 className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" />
                      Processing...
                    </>
                  ) : (
                    'Complete Registration'
                  )}
                </button>

                <button
                  onClick={handleSkipRoleSelection}
                  disabled={loading}
                  className="group relative w-full flex justify-center py-2 px-4 border border-gray-300 text-sm font-medium rounded-lg text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  Skip for now
                </button>
              </div>

              <p className="text-xs text-gray-500 text-center">
                You can change your role later in your profile settings
              </p>
            </div>
          )}

          <p className="text-center text-sm text-gray-600 mt-4">
            Already have an account?{' '}
            <Link to="/login" className="text-purple-600 font-medium hover:underline">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;