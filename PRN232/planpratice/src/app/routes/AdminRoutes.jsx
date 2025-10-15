import { Route, Navigate } from 'react-router-dom';
import AdminDashboard from '../pages/AdminDashboard';
import { AuthAPI } from '../components/APIService/AuthAPI';

// Admin Route Component
const AdminRoute = ({ children }) => {
    const user = AuthAPI.getUser();
    const isAuthenticated = AuthAPI.isAuthenticated();
    
    // Handle both numeric (2) and string ('Admin') roles
    let isAdmin = false;
    if (user && user.role !== undefined && user.role !== null) {
        const role = user.role.toString().toLowerCase();
        isAdmin = role === 'admin' || role === '2';
    }

    console.log('AdminRoute Debug:', {
        user,
        isAuthenticated,
        userRole: user?.role,
        isAdmin,
        roleType: typeof user?.role
    });

    return isAuthenticated && isAdmin ? children : <Navigate to="/" replace />;
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