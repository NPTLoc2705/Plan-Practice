import { Route, Navigate } from 'react-router-dom';
import AdminDashboard from '../pages/AdminDashboard';
import { AuthAPI } from '../components/APIService/AuthAPI';

// Admin Route Component
const AdminRoute = ({ children }) => {
    const user = AuthAPI.getUser();
    const isAuthenticated = AuthAPI.isAuthenticated();
    const isAdmin = user && user.role === 2;

    return isAuthenticated && isAdmin ? children : <Navigate to="/" replace />;
};

export const AdminRoutes = function () {
    return (
        <Route path="/admin" >
            <Route
                index
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