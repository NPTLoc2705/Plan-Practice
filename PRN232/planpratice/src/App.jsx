import './App.css'
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom'
import QuizManagement from './app/pages/QuizManagement.jsx'
import Login from './app/pages/Login';
import Register from './app/pages/Register';
import Profile from './app/pages/Profile';
import TeacherDashboard from './app/pages/TeacherDashboard';
import CreateQuiz from './app/pages/CreateQuiz.jsx';
import EditQuiz from './app/pages/EditQuiz.jsx';
import { AuthAPI } from './app/components/APIService/AuthAPI';
import Landing from './app/pages/Landing.jsx';
import HeaderBar from './app/components/HeaderBar.jsx';
import LessonPlanGenerator from './app/pages/LessonPlanner/Lesson.jsx';
import AdminDashboard from './app/pages/AdminDashboard.jsx';

// Protect routes
const ProtectedRoute = ({ children }) => {
    return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

const PublicRoute = ({ children }) => {
    return !AuthAPI.isAuthenticated() ? children : <Navigate to="/profile" replace />;
};

const AdminRoute = ({ children }) => {
    const user = AuthAPI.getUser();
    const isAuthenticated = AuthAPI.isAuthenticated();
    const isAdmin = user && user.role === 2;
    
    return isAuthenticated && isAdmin ? children : <Navigate to="/" replace />;
};

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
          path="/LessonPlanner"
          element = {
            <ProtectedRoute>
              <LessonPlanGenerator/>
            </ProtectedRoute>
          }
          ></Route>
        <Route 
          path="/" 
          element={
            // <ProtectedRoute>
              <Landing />
            // </ProtectedRoute>
          } 
        />
                    <Route
                        path="/quizmanagement"
                        element={
                            <ProtectedRoute>
                                <QuizManagement />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/quizmanagement/edit-quiz/:quizId"
                        element={
                            <ProtectedRoute>
                                <EditQuiz />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/quizmanagement/create-quiz"
                        element={
                            <ProtectedRoute>
                                <CreateQuiz />
                            </ProtectedRoute>
                        }
                    />
                    
                    {/* Admin Route */}
                    <Route
                        path="/admin/dashboard"
                        element={
                            <AdminRoute>
                                <AdminDashboard />
                            </AdminRoute>
                        }
                    />

                    {/* Landing (public) */}
                    <Route path="/" element={<Landing />} />

                    {/* Fallback */}
                    <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
            </Layout>
        </BrowserRouter>
    );
}

export default App;