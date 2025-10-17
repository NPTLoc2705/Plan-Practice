import './App.css'
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom'
import HeaderBar from './app/components/HeaderBar.jsx';
import { AuthRoutes } from './app/routes/AuthRoutes';
import { StudentRoutes } from './app/routes/StudentRoutes';
import { TeacherRoutes } from './app/routes/TeacherRoutes';
import { AdminRoutes } from './app/routes/AdminRoutes';
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
    const user = AuthAPI.getUser();
    
    if (!AuthAPI.isAuthenticated()) {
        return <Navigate to="/login" replace />;
    }
    
    if (user?.role) {
        const userRole = user.role.toString().toLowerCase();
        console.log('RoleBasedRedirect - User role:', userRole);
        switch (userRole) {
            case 'admin':
                return <Navigate to="/admin/dashboard" replace />;
            case 'teacher':
                return <Navigate to="/teacher" replace />;
            case 'student':
            default:
                return <Navigate to="/profile" replace />;
        }
    }
    
    return <Navigate to="/profile" replace />;
};

export default App;