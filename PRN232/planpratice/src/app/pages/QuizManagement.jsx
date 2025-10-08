import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { TeacherAPI } from '../components/APIService/TeacherAPI';
import { QuizAPI } from '../components/APIService/QuizAPI';
import { Plus, Edit, Trash2, FileQuestion, AlertCircle, CheckCircle } from 'lucide-react';
import './output.css';

const QuizManagement = () => {
    const [quizzes, setQuizzes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [deleteSuccess, setDeleteSuccess] = useState('');
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
            setQuizzes((prev) => prev.filter((quiz) => quiz.id !== quizId));
            setDeleteSuccess('Quiz deleted successfully!');
            setTimeout(() => setDeleteSuccess(''), 3000);
        } catch (err) {
            setError(`Failed to delete quiz: ${err.message}`);
            setTimeout(() => setError(null), 5000);
        }
    };

    const handleCreate = () => {
        navigate('/quizmanagement/create-quiz');
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 flex items-center justify-center">
                <div className="text-lg text-teal-600 font-medium">Loading quizzes...</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 py-8 px-4">
            <div className="max-w-7xl mx-auto">
                {/* Header Card */}
                <div className="bg-white rounded-2xl shadow-lg p-8 mb-6">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center gap-4">
                            <div className="w-14 h-14 bg-gradient-to-br from-teal-400 to-emerald-500 rounded-xl flex items-center justify-center">
                                <FileQuestion className="w-7 h-7 text-white" />
                            </div>
                            <div>
                                <h1 className="text-3xl font-bold text-gray-800">My Quizzes</h1>
                                <p className="text-gray-500 text-sm mt-1">
                                    Manage and organize all your quizzes
                                </p>
                            </div>
                        </div>
                        <button
                            onClick={handleCreate}
                            className="px-6 py-3 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all flex items-center gap-2 font-medium shadow-lg shadow-teal-200"
                        >
                            <Plus className="w-5 h-5" /> Create Quiz
                        </button>
                    </div>

                    {/* Success Message */}
                    {deleteSuccess && (
                        <div className="mt-4 bg-emerald-50 border-l-4 border-emerald-500 rounded-lg p-4 flex items-center gap-3">
                            <CheckCircle className="w-5 h-5 text-emerald-600 flex-shrink-0" />
                            <p className="text-emerald-700 font-medium">{deleteSuccess}</p>
                        </div>
                    )}

                    {/* Error Message */}
                    {error && (
                        <div className="mt-4 bg-red-50 border-l-4 border-red-500 rounded-lg p-4 flex items-start gap-3">
                            <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
                            <p className="text-red-700 font-medium">{error}</p>
                        </div>
                    )}
                </div>

                {/* Quizzes Content */}
                {quizzes.length === 0 ? (
                    <div className="bg-white rounded-2xl shadow-lg p-12 text-center">
                        <FileQuestion className="w-20 h-20 mx-auto mb-4 text-gray-300" />
                        <h3 className="text-xl font-semibold text-gray-700 mb-2">No quizzes yet</h3>
                        <p className="text-gray-500 mb-6">Create your first quiz to get started!</p>
                        <button
                            onClick={handleCreate}
                            className="px-6 py-3 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all inline-flex items-center gap-2 font-medium shadow-lg shadow-teal-200"
                        >
                            <Plus className="w-5 h-5" /> Create Your First Quiz
                        </button>
                    </div>
                ) : (
                    <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                        {quizzes.map((quiz) => (
                            <div
                                key={quiz.id}
                                className="bg-white rounded-2xl shadow-lg hover:shadow-xl transition-all border-2 border-transparent hover:border-teal-200 overflow-hidden group"
                            >
                                {/* Card Header */}
                                <div className="bg-gradient-to-r from-teal-500 to-emerald-500 p-6">
                                    <div className="flex items-start justify-between">
                                        <div className="flex-1">
                                            <h3 className="text-xl font-bold text-white mb-2 line-clamp-2">
                                                {quiz.title}
                                            </h3>
                                            <span className="inline-block px-3 py-1 bg-white bg-opacity-20 rounded-full text-xs font-medium text-white">
                                                ID: {quiz.id}
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                {/* Card Body */}
                                <div className="p-6">
                                    <p className="text-gray-600 text-sm line-clamp-3 mb-6 min-h-[60px]">
                                        {quiz.description || 'No description provided'}
                                    </p>

                                    {/* Action Buttons */}
                                    <div className="flex gap-3">
                                        <button
                                            onClick={() => handleEdit(quiz.id)}
                                            className="flex-1 px-4 py-2.5 bg-teal-50 text-teal-600 rounded-xl hover:bg-teal-100 transition-all flex items-center justify-center gap-2 font-medium"
                                        >
                                            <Edit className="w-4 h-4" />
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(quiz.id)}
                                            className="px-4 py-2.5 bg-red-50 text-red-600 rounded-xl hover:bg-red-100 transition-all flex items-center justify-center gap-2 font-medium"
                                        >
                                            <Trash2 className="w-4 h-4" />
                                        </button>
                                    </div>
                                </div>

                                {/* Hover Effect Indicator */}
                                <div className="h-1 bg-gradient-to-r from-teal-500 to-emerald-500 transform scale-x-0 group-hover:scale-x-100 transition-transform origin-left"></div>
                            </div>
                        ))}
                    </div>
                )}

                {/* Stats Footer */}
                {quizzes.length > 0 && (
                    <div className="mt-6 bg-white rounded-2xl shadow-lg p-6">
                        <div className="flex items-center justify-between text-sm">
                            <div className="flex items-center gap-2 text-gray-600">
                                <div className="w-2 h-2 bg-teal-500 rounded-full"></div>
                                <span className="font-medium">
                                    Total Quizzes: <span className="text-gray-800">{quizzes.length}</span>
                                </span>
                            </div>
                            <button
                                onClick={handleCreate}
                                className="text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1.5"
                            >
                                <Plus className="w-4 h-4" />
                                Add More
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default QuizManagement;