import { Route, Navigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';
import QuizList from '../pages/QuizList';
import TakeQuiz from '../pages/TakeQuiz';
import QuizResult from '../pages/QuizResult';
import QuizHistory from '../pages/QuizHistory';

// Protected Route Component for students
const StudentProtectedRoute = ({ children }) => {
    return AuthAPI.isAuthenticated() ? children : <Navigate to="/login" replace />;
};

export const StudentRoutes = function () {
    return (
        <Route path="/student" element={<StudentProtectedRoute />}>

            {/* Quiz Student Routes */}
            <Route
                path="quizzes"
                element={
                    <StudentProtectedRoute>
                        <QuizList />
                    </StudentProtectedRoute>
                }
            />
            <Route
                path="quiz/:id"
                element={
                    <StudentProtectedRoute>
                        <TakeQuiz />
                    </StudentProtectedRoute>
                }
            />
            <Route
                path="result/:id"
                element={
                    <StudentProtectedRoute>
                        <QuizResult />
                    </StudentProtectedRoute>
                }
            />
            <Route
                path="history"
                element={
                    <StudentProtectedRoute>
                        <QuizHistory />
                    </StudentProtectedRoute>
                }
            />
        </Route>
    );
};

export { StudentProtectedRoute };