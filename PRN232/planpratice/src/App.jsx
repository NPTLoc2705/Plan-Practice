import './App.css'
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom'
import HeaderBar from './app/components/HeaderBar.jsx';
import {
    AuthRoutes,
    StudentRoutes,
    TeacherRoutes,
    AdminRoutes,
    Landing,
} from './app/routes';

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

                    {/* Teacher Routes (including Quiz CRUD) */}
                    {TeacherRoutes()}

                    {/* Admin Routes */}
                    {AdminRoutes()}

                    {/* Landing Page */}
                    <Route path="/" element={<Landing />} />

          {/* Fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

export default App;
