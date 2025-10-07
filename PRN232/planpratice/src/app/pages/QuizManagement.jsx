import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { TeacherAPI } from '../components/APIService/TeacherAPI';
import { QuizAPI } from '../components/APIService/QuizAPI';
import { Plus, Edit, Trash2 } from 'lucide-react';
import './output.css'; 
const QuizManagement = () => {
    const [quizzes, setQuizzes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchQuizzes = async () => {
            try {
                const result = await TeacherAPI.getTeacherQuizzes();
                setQuizzes(result);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };
        fetchQuizzes();
    }, []);

    const handleEdit = (quizId) => {
        navigate(`/quizmanagement/edit-quiz/${quizId}`);
    };

    const handleDelete = async (quizId) => {
        if (!window.confirm('Are you sure you want to delete this quiz?')) return;

        try {
            await QuizAPI.deleteQuiz(quizId);

            // Update UI after delete
            setQuizzes((prev) => prev.filter((quiz) => quiz.id !== quizId));

            alert(`Quiz with ID ${quizId} deleted successfully.`);
        } catch (err) {
            alert(`Failed to delete quiz: ${err.message}`);
        }
    };

    const handleCreate = () => {
        navigate('/quizmanagement/create-quiz');
    };

    if (loading) return <div className="p-6 text-gray-500">Loading quizzes...</div>;
    if (error) return <div className="p-6 text-red-500">Error: {error}</div>;

    return (
        <div className="p-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-semibold text-gray-800">My Quizzes</h1>
                <button
                    onClick={handleCreate}
                    className="flex items-center bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-lg transition"
                >
                    <Plus className="w-5 h-5 mr-2" /> Create Quiz
                </button>
            </div>

            <div className="overflow-x-auto bg-white shadow-md rounded-xl border border-gray-200">
                <table className="min-w-full text-sm text-left text-gray-700">
                    <thead className="bg-gray-100 text-gray-900 font-medium">
                        <tr>
                            <th className="px-6 py-3">ID</th>
                            <th className="px-6 py-3">Title</th>
                            <th className="px-6 py-3">Description</th>
                            <th className="px-6 py-3 text-right">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {quizzes.length === 0 ? (
                            <tr>
                                <td colSpan="4" className="px-6 py-4 text-center text-gray-500">
                                    No quizzes found.
                                </td>
                            </tr>
                        ) : (
                            quizzes.map((quiz) => (
                                <tr key={quiz.id} className="border-b hover:bg-gray-50">
                                    <td className="px-6 py-3">{quiz.id}</td>
                                    <td className="px-6 py-3 font-medium">{quiz.title}</td>
                                    <td className="px-6 py-3">{quiz.description}</td>
                                    <td className="px-6 py-3 text-right space-x-2">
                                        <button
                                            onClick={() => handleEdit(quiz.id)}
                                            className="text-indigo-600 hover:text-indigo-800 transition"
                                        >
                                            <Edit className="inline w-5 h-5" />
                                        </button>
                                        <button
                                            onClick={() => handleDelete(quiz.id)}
                                            className="text-red-600 hover:text-red-800 transition"
                                        >
                                            <Trash2 className="inline w-5 h-5" />
                                        </button>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default QuizManagement;
