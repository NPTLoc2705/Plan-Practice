import { Route, Navigate } from 'react-router-dom';
import Login from '../pages/Login';
import Register from '../pages/Register';
import Profile from '../pages/Profile';
import { AuthAPI } from '../components/APIService/AuthAPI';

// Public Route Component for authentication
const PublicRoute = ({ children }) => {
    return !AuthAPI.isAuthenticated() ? children : <Navigate to="/profile" replace />;
};

const ProtectedRoute = ({ children }) => {
    return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

export const AuthRoutes = function () {
    return (
        <>
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
        </>
    );
};

export { PublicRoute, ProtectedRoute };