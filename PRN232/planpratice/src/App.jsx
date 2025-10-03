import './App.css'
import { BrowserRouter, Routes, Route, Link, Navigate } from 'react-router-dom'
import QuizManagement from './app/pages/QuizManagement.jsx'
import Login from './app/pages/Login';
import Register from './app/pages/Register';
import Profile from './app/pages/Profile';
import TeacherDashboard from './app/pages/TeacherDashboard';
import { AuthAPI } from './app/components/APIService/AuthAPI';
import Landing from './app/pages/Landing.jsx';
import HeaderBar from './app/components/HeaderBar.jsx';
import QuizList from './app/pages/QuizList.jsx';
import TakeQuiz from './app/pages/TakeQuiz.jsx';
import QuizResult from './app/pages/QuizResult.jsx';
import QuizHistory from './app/pages/QuizHistory.jsx';

// Protect routes
const ProtectedRoute = ({ children }) => {
  return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

const PublicRoute = ({ children }) => {
  return !AuthAPI.isAuthenticated() ? children : <Navigate to="/profile" replace />;
};
// Student Layout Component
const StudentLayout = () => {
  const handleLogout = () => {
    AuthAPI.logout();
    window.location.href = '/login';
  };

  return (
    <div className="student-layout">
      <nav className="student-nav" style={{ 
        display: 'flex', 
        justifyContent: 'space-between',
        alignItems: 'center',
        padding: '12px',
        backgroundColor: '#f0f0f0',
        marginBottom: '20px'
      }}>
        <div style={{ display: 'flex', gap: 12 }}>
          <Link to="/student/quizzes">Available Quizzes</Link>
          <Link to="/student/history">My History</Link>
        </div>
        <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
          <span>Welcome, {AuthAPI.getUser()?.name || 'Student'}</span>
          <button onClick={handleLogout} style={{ 
            padding: '6px 12px',
            backgroundColor: '#dc3545',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer'
          }}>
            Logout
          </button>
        </div>
      </nav>
      <div className="student-content" style={{ padding: '0 20px' }}>
        <Routes>
          <Route index element={<Navigate to="quizzes" replace />} />
          <Route path="quizzes" element={<QuizList />} />
          <Route path="quiz/:id" element={<TakeQuiz />} />
          <Route path="result/:id" element={<QuizResult />} />
          <Route path="history" element={<QuizHistory />} />
        </Routes>
      </div>
    </div>
  );
};
function App() {
  return (
    <BrowserRouter>
     <HeaderBar/>
     

      <Routes>
        {/* Public pages */}
        <Route 
          path="/login" 
          element={
            <PublicRoute>
              <Login />
            </PublicRoute>
          } 
        />
        <Route 
          path="/register" 
          element={
            <PublicRoute>
              <Register />
            </PublicRoute>
          } 
        />

        {/* Protected pages */}
        <Route 
          path="/profile" 
          element={
            <ProtectedRoute>
              <Profile />
            </ProtectedRoute>
          } 
        />
        <Route 
          path="/teacher" 
          element={
            // <ProtectedRoute>
              <TeacherDashboard />
            // </ProtectedRoute>
          } 
        />  
        <Route 
          path="/" 
          element={
            // <ProtectedRoute>
              <Landing />
            // </ProtectedRoute>
          } 
        />


        {/* Other routes */}
        <Route path="/" element={<div style={{ padding: 16 }}><h1>Home</h1><p>Welcome.</p></div>} />
        <Route path="/quiz/*" element={<QuizManagement />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
