import { Route, Navigate } from 'react-router-dom';
import AdminDashboard from '../pages/AdminDashboard';
import { AuthAPI } from '../components/APIService/AuthAPI';

// Admin Route Component
const AdminRoute = ({ children }) => {
    // Check if user is authenticated
    if (!AuthAPI.isAuthenticated()) {
        return <Navigate to="/login" replace />;
    }
    
    // Check if user has Admin role
    if (!AuthAPI.isAdmin()) {
        console.warn('Access denied: User does not have Admin role');
        return <Navigate to="/" replace />;
    }
    
    return children;
};

export const AdminRoutes = function () {
    return (
        <Route path="/admin">
            <Route
                path="dashboard"
                element={
                    <AdminRoute>
                        <AdminDashboard />
                    </AdminRoute>
                }
            />
        </Route>
    );
};

export { AdminRoute };