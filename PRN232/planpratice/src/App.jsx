
import './App.css'
import Login from './app/pages/Login';
import Register from './app/pages/Register';
import Profile from './app/pages/Profile';
import Landing from './app/pages/Landing';
import { Routes, Route, Navigate } from 'react-router';
import { AuthAPI } from './app/components/APIService/AuthAPI';


const ProtectedRoute = ({ children }) => {
  return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

const PublicRoute = ({ children }) => {
  return !AuthAPI.isAuthenticated() ? children : <Navigate to="/dashboard" replace />;
};



function App() {
  return (
    <div className="App">
      <Routes>
        <Route
          path="/"
          element={
            <Landing />
          }
        />
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
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <Profile />
            </ProtectedRoute>
          }
        />
        <Route path="/" element={<Navigate to="/" replace />} />
        <Route path="*" element={<Navigate to="/profile" replace />} />
      </Routes>
    </div>
  );
}

export default App
