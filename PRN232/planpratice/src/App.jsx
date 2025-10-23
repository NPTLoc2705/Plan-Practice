import './App.css'
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom'
import HeaderBar from './app/components/HeaderBar.jsx';
import { AuthRoutes } from './app/routes/AuthRoutes';
import { StudentRoutes } from './app/routes/StudentRoutes';
import { TeacherRoutes } from './app/routes/TeacherRoutes';
import { AdminRoutes } from './app/routes/AdminRoutes';
import { AuthAPI } from './app/components/APIService/AuthAPI';
import Landing from './app/pages/Landing.jsx';

// Component to conditionally render HeaderBar
const Layout = ({ children }) => {
    const location = useLocation();
    const hideHeaderRoutes = ['/admin/dashboard']; // Add more routes here if needed

    return (
        <>
            {!hideHeaderRoutes.includes(location.pathname) && <HeaderBar />}
            {children}
        </>
    );
};

function App() {
    return (
        <BrowserRouter>
            <Layout>
                <Routes>
                    {/* Authentication Routes */}
                    {AuthRoutes()}

                    {/* Student Routes */}
                    {StudentRoutes()}

                    {/* Teacher Routes */}
                    {TeacherRoutes()}

                    {/* Admin Routes */}
                    {AdminRoutes()}

                    {/* Landing Page */}
                    <Route path="/" element={<Landing />} />

                    {/* Auto-redirect based on role */}
                    <Route 
                        path="/dashboard" 
                        element={<RoleBasedRedirect />} 
                    />

                    {/* Fallback */}
                    <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
            </Layout>
        </BrowserRouter>
    );
}

// Role-based redirect component
const RoleBasedRedirect = () => {
    if (!AuthAPI.isAuthenticated()) {
        return <Navigate to="/login" replace />;
    }
    
    // Use JWT token to determine role
    const userRole = AuthAPI.getUserRole();
    
    if (!userRole) {
        console.warn('No role found in token, redirecting to home');
        return <Navigate to="/" replace />;
    }
    
    console.log('RoleBasedRedirect - User role from JWT:', userRole);
    
    // Redirect based on role from JWT
    if (AuthAPI.isAdmin()) {
        return <Navigate to="/admin/dashboard" replace />;
    } else if (AuthAPI.isTeacher()) {
        return <Navigate to="/teacher" replace />;
    } else if (AuthAPI.isStudent()) {
        return <Navigate to="/profile" replace />;
    }
    
    // Default fallback
    return <Navigate to="/" replace />;
};

export default App;