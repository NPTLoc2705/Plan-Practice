import { Route, Navigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';
import QuizList from '../pages/QuizList';
import TakeQuiz from '../pages/TakeQuiz';
import QuizResult from '../pages/QuizResult';
import QuizHistory from '../pages/QuizHistory';

import TakeQuizWithOTP from "../pages/TakeQuizWithOTP";

// Protected Route Component for students
const StudentProtectedRoute = ({ children }) => {
    // Check if user is authenticated
    if (!AuthAPI.isAuthenticated()) {
        return <Navigate to="/login" replace />;
    }
    
    // Check if user has Student role
    if (!AuthAPI.isStudent()) {
        console.warn('Access denied: User does not have Student role');
        return <Navigate to="/" replace />;
    }
    
    return children;
};

export const StudentRoutes = function () {
    return (
        <Route path="/student">

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
             <Route 
                path="quiz/otp" 
                element={
                    <StudentProtectedRoute>
                        <TakeQuizWithOTP />
                    </StudentProtectedRoute>
                } 
            /> 
      
        </Route>
        
    );
};

export { StudentProtectedRoute };