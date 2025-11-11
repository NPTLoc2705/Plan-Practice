import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { QuizAPI } from '../components/APIService/QuizAPI';
import { LessonPlannerAPI } from '../components/APIService/LessonPlannerAPI';
import { Plus, Edit, Trash2, FileQuestion, AlertCircle, CheckCircle, BookOpen, ChevronDown, Save, Sparkles, Zap } from 'lucide-react';
import './output.css';

const QuizManagement = () => {
    const [quizzes, setQuizzes] = useState([]);
    const [lessonPlanners, setLessonPlanners] = useState([]);
    const [selectedLessonPlanner, setSelectedLessonPlanner] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [deleteSuccess, setDeleteSuccess] = useState('');
    const [dropdownOpen, setDropdownOpen] = useState(false);
    
    // ========== BỔ SUNG: AI STATE ==========
    const [aiLoading, setAiLoading] = useState(false);
    const [showAIModal, setShowAIModal] = useState(false);
    const [aiQuestionCount, setAiQuestionCount] = useState(5);
    const [aiQuizTitle, setAiQuizTitle] = useState('');
    const [aiQuizDescription, setAiQuizDescription] = useState('');
    
    const navigate = useNavigate();

    // Fetch lesson planners on component mount
    useEffect(() => {
        const fetchLessonPlanners = async () => {
            try {
                const result = await LessonPlannerAPI.getMyLessonPlanners();
                if (result.success && Array.isArray(result.data)) {
                    setLessonPlanners(result.data);
                    if (result.data.length > 0) {
                        setSelectedLessonPlanner(result.data[0]);
                    }
                }
            } catch (err) {
                setError(`Failed to load lesson planners: ${err.message}`);
            }
        };
        fetchLessonPlanners();
    }, []);

    // Fetch quizzes when lesson planner changes
    useEffect(() => {
        const fetchQuizzes = async () => {
            if (!selectedLessonPlanner) {
                setQuizzes([]);
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                const result = await QuizAPI.getQuizzesByLessonPlannerId(selectedLessonPlanner.id);
                setQuizzes(result);
            } catch (err) {
                setError(`Failed to load quizzes: ${err.message}`);
                setQuizzes([]);
            } finally {
                setLoading(false);
            }
        };
        fetchQuizzes();
    }, [selectedLessonPlanner]);

    const handleLessonPlannerSelect = (planner) => {
        setSelectedLessonPlanner(planner);
        setDropdownOpen(false);
        setError(null);
        // Reset AI form when changing lesson planner
        setAiQuizTitle('');
        setAiQuizDescription('');
    };

    // ========== BỔ SUNG: AI GENERATION FUNCTION ==========
    const handleAIGenerate = async () => {
        if (!selectedLessonPlanner) {
            setError('Please select a lesson planner first.');
            return;
        }

        setAiLoading(true);
        setError('');
        setDeleteSuccess('');
        setShowAIModal(false);

        try {
            const aiPayload = {
                lessonPlannerId: selectedLessonPlanner.id,
                title: aiQuizTitle || `AI Quiz for ${selectedLessonPlanner.title}`,
                description: aiQuizDescription || 'Auto-generated quiz from lesson content',
                numberOfQuestions: aiQuestionCount,
            };

            const response = await QuizAPI.generateQuizWithAI(aiPayload);

            if (response.success && response.data) {
                setDeleteSuccess(`✨ Successfully generated quiz with ${response.data.questionsCount || aiQuestionCount} questions!`);
                
                // Refresh quiz list
                const updatedQuizzes = await QuizAPI.getQuizzesByLessonPlannerId(selectedLessonPlanner.id);
                setQuizzes(updatedQuizzes);

                // Reset form
                setAiQuizTitle('');
                setAiQuizDescription('');
                setAiQuestionCount(5);

                // Auto-dismiss success message after 5 seconds
                setTimeout(() => setDeleteSuccess(''), 5000);

                // Optional: Navigate to edit the newly created quiz
                // navigate(`/teacher/quiz/edit/${response.data.id}`);
            } else {
                throw new Error(response.message || 'Failed to generate quiz');
            }
        } catch (err) {
            console.error('Error generating quiz with AI:', err);
            setError(err.message || 'Failed to generate quiz with AI. Please try again.');
            setTimeout(() => setError(null), 5000);
        } finally {
            setAiLoading(false);
        }
    };

    const handleEdit = (quizId) => {
        navigate(`/teacher/quiz/edit/${quizId}`);
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
        const createUrl = selectedLessonPlanner
            ? `/teacher/quiz/create?lessonPlannerId=${selectedLessonPlanner.id}`
            : '/teacher/quiz/create';
        navigate(createUrl);
    };

    if (loading && lessonPlanners.length === 0) {
        return (
            <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 flex items-center justify-center">
                <div className="text-lg text-teal-600 font-medium">Loading lesson planners...</div>
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
                                <h1 className="text-3xl font-bold text-gray-800">Quiz Management</h1>
                                <p className="text-gray-500 text-sm mt-1">
                                    Manage quizzes by lesson planner
                                </p>
                            </div>
                        </div>
                        
                        {/* ========== BỔ SUNG: NÚT CREATE VÀ AI ========== */}
                        <div className="flex gap-3">
                            {/* Nút Generate with AI */}
                            <button
                                onClick={() => setShowAIModal(true)}
                                disabled={!selectedLessonPlanner || aiLoading}
                                className="px-6 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-xl hover:from-purple-600 hover:to-pink-600 transition-all flex items-center gap-2 font-medium shadow-lg shadow-purple-200 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                {aiLoading ? (
                                    <>
                                        <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                        Generating...
                                    </>
                                ) : (
                                    <>
                                        <Sparkles className="w-5 h-5" /> Generate with AI
                                    </>
                                )}
                            </button>

                            {/* Nút Create Manual */}
                            <button
                                onClick={handleCreate}
                                disabled={!selectedLessonPlanner}
                                className="px-6 py-3 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all flex items-center gap-2 font-medium shadow-lg shadow-teal-200 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <Plus className="w-5 h-5" /> Create Quiz
                            </button>
                        </div>
                    </div>

                    {/* Lesson Planner Selection */}
                    <div className="mt-6">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Select Lesson Planner
                        </label>
                        <div className="relative">
                            <button
                                onClick={() => setDropdownOpen(!dropdownOpen)}
                                className="w-full max-w-md px-4 py-3 bg-white border border-gray-300 rounded-xl shadow-sm text-left hover:border-teal-400 focus:border-teal-500 focus:ring-2 focus:ring-teal-200 transition-all flex items-center justify-between"
                            >
                                <div className="flex items-center gap-3">
                                    <BookOpen className="w-5 h-5 text-teal-600" />
                                    <span className="text-gray-700">
                                        {selectedLessonPlanner ? selectedLessonPlanner.title : 'Choose a lesson planner...'}
                                    </span>
                                </div>
                                <ChevronDown className={`w-5 h-5 text-gray-400 transform transition-transform ${dropdownOpen ? 'rotate-180' : ''}`} />
                            </button>

                            {dropdownOpen && (
                                <div className="absolute z-10 w-full max-w-md mt-1 bg-white border border-gray-200 rounded-xl shadow-lg max-h-60 overflow-y-auto">
                                    {lessonPlanners.length === 0 ? (
                                        <div className="p-4 text-center text-gray-500">
                                            No lesson planners found
                                        </div>
                                    ) : (
                                        lessonPlanners.map((planner) => (
                                            <button
                                                key={planner.id}
                                                onClick={() => handleLessonPlannerSelect(planner)}
                                                className={`w-full px-4 py-3 text-left hover:bg-teal-50 transition-colors border-b border-gray-100 last:border-b-0 ${selectedLessonPlanner?.id === planner.id ? 'bg-teal-50 text-teal-700' : 'text-gray-700'
                                                    }`}
                                            >
                                                <div className="font-medium">{planner.title}</div>
                                                {planner.description && (
                                                    <div className="text-sm text-gray-500 truncate">{planner.description}</div>
                                                )}
                                            </button>
                                        ))
                                    )}
                                </div>
                            )}
                        </div>
                    </div>

                    {/* ========== AI LOADING STATE ========== */}
                    {aiLoading && (
                        <div className="mt-6 bg-purple-50 border-2 border-purple-200 rounded-xl p-6 flex items-center gap-4">
                            <div className="relative">
                                <Sparkles className="w-8 h-8 text-purple-500 animate-pulse" />
                                <div className="absolute inset-0 w-8 h-8 border-4 border-purple-300 border-t-transparent rounded-full animate-spin" />
                            </div>
                            <div>
                                <p className="text-purple-800 font-semibold">AI is generating your quiz...</p>
                                <p className="text-purple-600 text-sm">This may take 5-15 seconds. Please wait.</p>
                            </div>
                        </div>
                    )}

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
                {!selectedLessonPlanner ? (
                    <div className="bg-white rounded-2xl shadow-lg p-12 text-center">
                        <BookOpen className="w-20 h-20 mx-auto mb-4 text-gray-300" />
                        <h3 className="text-xl font-semibold text-gray-700 mb-2">Select a lesson planner</h3>
                        <p className="text-gray-500">Choose a lesson planner to view and manage its quizzes.</p>
                    </div>
                ) : loading ? (
                    <div className="bg-white rounded-2xl shadow-lg p-12 text-center">
                        <div className="text-lg text-teal-600 font-medium">Loading quizzes...</div>
                    </div>
                ) : quizzes.length === 0 ? (
                    <div className="bg-white rounded-2xl shadow-lg p-12 text-center">
                        <FileQuestion className="w-20 h-20 mx-auto mb-4 text-gray-300" />
                        <h3 className="text-xl font-semibold text-gray-700 mb-2">No quizzes for this lesson planner</h3>
                        <p className="text-gray-500 mb-6">Create your first quiz for "{selectedLessonPlanner.title}"!</p>
                        <div className="flex justify-center gap-3">
                            <button
                                onClick={() => setShowAIModal(true)}
                                className="px-6 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-xl hover:from-purple-600 hover:to-pink-600 transition-all inline-flex items-center gap-2 font-medium shadow-lg shadow-purple-200"
                            >
                                <Sparkles className="w-5 h-5" /> Generate with AI
                            </button>
                            <button
                                onClick={handleCreate}
                                className="px-6 py-3 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all inline-flex items-center gap-2 font-medium shadow-lg shadow-teal-200"
                            >
                                <Plus className="w-5 h-5" /> Create Manually
                            </button>
                        </div>
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
                                            <div className="flex gap-2">
                                                <span className="inline-block px-3 py-1 bg-white bg-opacity-20 rounded-full text-xs font-medium text-white">
                                                    ID: {quiz.id}
                                                </span>
                                                {quiz.createdAt && (
                                                    <span className="inline-block px-3 py-1 bg-white bg-opacity-20 rounded-full text-xs font-medium text-white">
                                                        {new Date(quiz.createdAt).toLocaleDateString()}
                                                    </span>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                {/* Card Body */}
                                <div className="p-6">
                                    <p className="text-gray-600 text-sm line-clamp-3 mb-6 min-h-[60px]">
                                        {quiz.description || 'No description provided'}
                                    </p>

                                    {/* Quiz Metadata */}
                                    <div className="flex items-center justify-between mb-4 text-xs text-gray-500">
                                        <span>
                                            Lesson: {selectedLessonPlanner.title}
                                        </span>
                                        <span>
                                            {quiz.totalQuestion || 0} questions
                                        </span>
                                    </div>

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
                {selectedLessonPlanner && quizzes.length > 0 && (
                    <div className="mt-6 bg-white rounded-2xl shadow-lg p-6">
                        <div className="flex items-center justify-between text-sm">
                            <div className="flex items-center gap-2 text-gray-600">
                                <div className="w-2 h-2 bg-teal-500 rounded-full"></div>
                                <span className="font-medium">
                                    Quizzes in "{selectedLessonPlanner.title}": <span className="text-gray-800">{quizzes.length}</span>
                                </span>
                            </div>
                            <div className="flex gap-3">
                                <button
                                    onClick={() => setShowAIModal(true)}
                                    className="text-purple-600 hover:text-purple-700 font-medium flex items-center gap-1.5"
                                >
                                    <Sparkles className="w-4 h-4" />
                                    Generate with AI
                                </button>
                                <button
                                    onClick={handleCreate}
                                    className="text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1.5"
                                >
                                    <Plus className="w-4 h-4" />
                                    Add Manually
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>

            {/* ========== AI MODAL ========== */}
            {showAIModal && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                    <div className="bg-white rounded-2xl shadow-2xl max-w-lg w-full p-8 animate-in fade-in zoom-in duration-200">
                        <div className="flex items-center gap-3 mb-6">
                            <div className="w-12 h-12 bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl flex items-center justify-center">
                                <Zap className="w-6 h-6 text-white" />
                            </div>
                            <h3 className="text-2xl font-bold text-gray-800">Generate Quiz with AI</h3>
                        </div>

                        <div className="bg-purple-50 border-l-4 border-purple-500 rounded-lg p-4 mb-6">
                            <p className="text-purple-800 text-sm">
                                <strong>✨ AI will analyze:</strong> "{selectedLessonPlanner?.title}"
                            </p>
                            <p className="text-purple-700 text-xs mt-1">
                                and automatically create quiz with questions & answers!
                            </p>
                        </div>

                        <div className="space-y-4 mb-6">
                            <div>
                                <label className="block text-sm font-semibold text-gray-700 mb-2">
                                    Quiz Title (Optional)
                                </label>
                                <input
                                    type="text"
                                    value={aiQuizTitle}
                                    onChange={(e) => setAiQuizTitle(e.target.value)}
                                    placeholder={`AI Quiz for ${selectedLessonPlanner?.title}`}
                                    className="w-full border-2 border-gray-200 rounded-xl px-4 py-3 focus:border-purple-500 focus:outline-none transition-colors"
                                />
                            </div>

                            <div>
                                <label className="block text-sm font-semibold text-gray-700 mb-2">
                                    Description (Optional)
                                </label>
                                <textarea
                                    value={aiQuizDescription}
                                    onChange={(e) => setAiQuizDescription(e.target.value)}
                                    placeholder="Auto-generated quiz from lesson content"
                                    rows={3}
                                    className="w-full border-2 border-gray-200 rounded-xl px-4 py-3 focus:border-purple-500 focus:outline-none transition-colors resize-none"
                                />
                            </div>

                            <div>
                                <label className="block text-sm font-semibold text-gray-700 mb-2">
                                    Number of Questions
                                </label>
                                <input
                                    type="number"
                                    min="1"
                                    max="20"
                                    value={aiQuestionCount}
                                    onChange={(e) => setAiQuestionCount(parseInt(e.target.value) || 5)}
                                    className="w-full border-2 border-gray-200 rounded-xl px-4 py-3 focus:border-purple-500 focus:outline-none transition-colors text-lg font-semibold text-center"
                                />
                                <p className="text-xs text-gray-500 mt-2 text-center">Choose between 1-20 questions</p>
                            </div>
                        </div>

                        <div className="flex gap-3">
                            <button
                                type="button"
                                onClick={() => {
                                    setShowAIModal(false);
                                    setAiQuizTitle('');
                                    setAiQuizDescription('');
                                    setAiQuestionCount(5);
                                }}
                                className="flex-1 px-4 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-all font-medium"
                            >
                                Cancel
                            </button>
                            <button
                                type="button"
                                onClick={handleAIGenerate}
                                className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-xl hover:from-purple-600 hover:to-pink-600 transition-all flex items-center justify-center gap-2 font-medium shadow-lg shadow-purple-200"
                            >
                                <Sparkles className="w-5 h-5" />
                                Generate Now
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default QuizManagement;