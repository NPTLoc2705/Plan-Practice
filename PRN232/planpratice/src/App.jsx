import './App.css'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import QuizManagement from './app/pages/QuizManagement.jsx'
import Login from './app/pages/Login';
import Register from './app/pages/Register';
import Profile from './app/pages/Profile';
import TeacherDashboard from './app/pages/TeacherDashboard';
import CreateQuiz from './app/pages/CreateQuiz.jsx'; // ✅ New page
import EditQuiz from './app/pages/EditQuiz.jsx'; // ✅ New page
import { AuthAPI } from './app/components/APIService/AuthAPI';
import Landing from './app/pages/Landing.jsx';
import HeaderBar from './app/components/HeaderBar.jsx';
import LessonPlanGenerator from './app/pages/LessonPlanner/Lesson.jsx';
// Protect routes
const ProtectedRoute = ({ children }) => {
    return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

const PublicRoute = ({ children }) => {
    return !AuthAPI.isAuthenticated() ? children : <Navigate to="/profile" replace />;
};

function App() {
    return (
        <BrowserRouter>
            <HeaderBar />

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
                            <QuizManagement /> {/* ✅ Dashboard for Teacher */}
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
                            <CreateQuiz /> {/* ✅ Separate Create Quiz page */}
                        </ProtectedRoute>
                    }
                />

                {/* Landing (public) */}
                <Route path="/" element={<Landing />} />

                {/* Fallback */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
