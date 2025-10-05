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
// Protect routes
const ProtectedRoute = ({ children }) => {
  return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

const PublicRoute = ({ children }) => {
  return !AuthAPI.isAuthenticated() ? children : <Navigate to="/profile" replace />;
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
            <ProtectedRoute>
              <TeacherDashboard />
            </ProtectedRoute>
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
