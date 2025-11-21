import { Route, Navigate } from 'react-router-dom';
import TeacherDashboard from '../pages/TeacherDashboard';
import QuizManagement from '../pages/QuizManagement';
import CreateQuiz from '../pages/CreateQuiz';
import EditQuiz from '../pages/EditQuiz';
import LessonPlanGenerator from '../pages/LessonPlanner/Lesson';
import EditLesson from '../pages/LessonPlanner/EditLesson';
import ViewLesson from '../pages/LessonPlanner/ViewLesson';
import LessonSettings from '../pages/LessonPlanner/LessonSettings';
import { AuthAPI } from '../components/APIService/AuthAPI';

import TeacherOTPManager from "../pages/TeacherOTPManager";

// Protected Route Component for teachers
const TeacherProtectedRoute = ({ children }) => {
    // Check if user is authenticated
    if (!AuthAPI.isAuthenticated()) {
        return <Navigate to="/login" replace />;
    }
    
    // Check if user has Teacher role
    if (!AuthAPI.isTeacher()) {
        console.warn('Access denied: User does not have Teacher role');
        return <Navigate to="/" replace />;
    }
    
    return children;
};

export const TeacherRoutes = function () {
    return (
        <Route path="/teacher">
            <Route
                index
                element={
                    <TeacherProtectedRoute>
                        <TeacherDashboard />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="Lesson/planner"
                element={
                    <TeacherProtectedRoute>
                        <LessonPlanGenerator />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="Lesson/planner/edit/:id"
                element={
                    <TeacherProtectedRoute>
                        <EditLesson />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="Lesson/planner/:id/view"
                element={
                    <TeacherProtectedRoute>
                        <ViewLesson />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="Lesson/planner/settings"
                element={
                    <TeacherProtectedRoute>
                        <LessonSettings />
                    </TeacherProtectedRoute>
                }
            />
            {/* Quiz CRUD Routes */}
            <Route
                path="quiz"
                element={
                    <TeacherProtectedRoute>
                        <QuizManagement />
                    </TeacherProtectedRoute>
                }
            />
            {/* Teacher routes */}
        <Route path="otp-manager" element={<TeacherOTPManager />} />
            <Route
                path="quiz/create"
                element={
                    <TeacherProtectedRoute>
                        <CreateQuiz />
                    </TeacherProtectedRoute>
                }
            />
            <Route
                path="quiz/edit/:quizId"
                element={
                    <TeacherProtectedRoute>
                        <EditQuiz />
                    </TeacherProtectedRoute>
                }
            />
        </Route>
    );
};

export { TeacherProtectedRoute };