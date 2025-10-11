import { Route, Navigate } from 'react-router-dom';
import TeacherDashboard from '../pages/TeacherDashboard';
import QuizManagement from '../pages/QuizManagement';
import CreateQuiz from '../pages/CreateQuiz';
import EditQuiz from '../pages/EditQuiz';
import LessonPlanGenerator from '../pages/LessonPlanner/Lesson';
import { AuthAPI } from '../components/APIService/AuthAPI';

// Protected Route Component for teachers
const TeacherProtectedRoute = ({ children }) => {
    return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

export const TeacherRoutes = function () {
    return (
        <>
            <Route
                path="/teacher"
                element={
                    <TeacherProtectedRoute>
                        <TeacherDashboard />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="/LessonPlanner"
                element={
                    <TeacherProtectedRoute>
                        <LessonPlanGenerator />
                    </TeacherProtectedRoute>
                }
            />
            {/* Quiz CRUD Routes */}
            <Route
                path="/quizmanagement"
                element={
                    <TeacherProtectedRoute>
                        <QuizManagement />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="/quizmanagement/create-quiz"
                element={
                    <TeacherProtectedRoute>
                        <CreateQuiz />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="/quizmanagement/edit-quiz/:quizId"
                element={
                    <TeacherProtectedRoute>
                        <EditQuiz />
                    </TeacherProtectedRoute>
                }
            />
        </>
    );
};

export { TeacherProtectedRoute };