import { Route, Navigate } from 'react-router-dom';
import Landing from '../pages/Landing';
import { AuthAPI } from '../components/APIService/AuthAPI';

// Protected Route Component for students
const StudentProtectedRoute = ({ children }) => {
    return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

export const StudentRoutes = function () {
    return (
        <>
            {/* Student specific routes can be added here */}
            {/* For now, we're keeping it empty as Landing is handled in main App */}
        </>
    );
};

export { StudentProtectedRoute };