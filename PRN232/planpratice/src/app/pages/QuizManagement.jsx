import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { QuizAPI } from '../components/APIService/QuizAPI';
import { LessonPlannerAPI } from '../components/APIService/LessonPlannerAPI';
import { 
    Plus, Edit, Trash2, FileQuestion, AlertCircle, CheckCircle, 
    BookOpen, ChevronDown, Save, Sparkles, Zap, Search, Filter,
    MoreVertical, Clock, Users, Trophy, BarChart3, Grid, List,
    Calendar, Eye, Copy, Download, Share2, ChevronRight, X
} from 'lucide-react';
import './output.css';

const QuizManagement = () => {
    const [quizzes, setQuizzes] = useState([]);
    const [lessonPlanners, setLessonPlanners] = useState([]);
    const [selectedLessonPlanner, setSelectedLessonPlanner] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [deleteSuccess, setDeleteSuccess] = useState('');
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const [viewMode, setViewMode] = useState('grid'); // grid or list
    const [searchTerm, setSearchTerm] = useState('');
    const [lessonPlannerSearch, setLessonPlannerSearch] = useState(''); // New state for lesson planner search
    const [selectedQuiz, setSelectedQuiz] = useState(null);
    const [sidebarCollapsed, setSidebarCollapsed] = useState(false);
    
    // AI STATE
    const [aiLoading, setAiLoading] = useState(false);
    const [showAIModal, setShowAIModal] = useState(false);
    const [aiQuestionCount, setAiQuestionCount] = useState(5);
    const [aiQuizTitle, setAiQuizTitle] = useState('');
    const [aiQuizDescription, setAiQuizDescription] = useState('');
    
    const navigate = useNavigate();

    // Fetch lesson planners
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

    // Fetch quizzes
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
        setAiQuizTitle('');
        setAiQuizDescription('');
    };

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
                
                // Dispatch event to refresh coin balance in header
                window.dispatchEvent(new Event('refreshCoinBalance'));

                const updatedQuizzes = await QuizAPI.getQuizzesByLessonPlannerId(selectedLessonPlanner.id);
                setQuizzes(updatedQuizzes);

                setAiQuizTitle('');
                setAiQuizDescription('');
                setAiQuestionCount(5);

                setTimeout(() => setDeleteSuccess(''), 5000);
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

    // Filter quizzes based on search
    const filteredQuizzes = quizzes.filter(quiz => 
        quiz.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        quiz.description?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    // Filter lesson planners based on search
    const filteredLessonPlanners = lessonPlanners.filter(planner =>
        planner.title.toLowerCase().includes(lessonPlannerSearch.toLowerCase()) ||
        planner.description?.toLowerCase().includes(lessonPlannerSearch.toLowerCase())
    );

    // Calculate stats
    const stats = {
        total: quizzes.length,
        totalQuestions: quizzes.reduce((sum, quiz) => sum + (quiz.totalQuestion || 0), 0),
        avgQuestions: quizzes.length > 0 ? Math.round(quizzes.reduce((sum, quiz) => sum + (quiz.totalQuestion || 0), 0) / quizzes.length) : 0,
        recentQuizzes: quizzes.filter(quiz => {
            const date = new Date(quiz.createdAt);
            const weekAgo = new Date();
            weekAgo.setDate(weekAgo.getDate() - 7);
            return date > weekAgo;
        }).length
    };

    if (loading && lessonPlanners.length === 0) {
        return (
            <div className="min-h-screen bg-gray-50 flex items-center justify-center">
                <div className="flex flex-col items-center gap-4">
                    <div className="w-16 h-16 border-4 border-indigo-600 border-t-transparent rounded-full animate-spin"></div>
                    <p className="text-gray-600 font-medium">Loading your content...</p>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 flex">
            {/* Sidebar */}
            <div className={`${sidebarCollapsed ? 'w-20' : 'w-64'} bg-white border-r border-gray-200 transition-all duration-300 flex flex-col h-screen`}>
                <div className="p-4 border-b border-gray-200">
                    <div className="flex items-center justify-between">
                        <div className={`flex items-center gap-3 ${sidebarCollapsed ? 'justify-center' : ''}`}>
                            <div className="w-10 h-10 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg flex items-center justify-center">
                                <FileQuestion className="w-5 h-5 text-white" />
                            </div>
                            {!sidebarCollapsed && (
                                <span className="font-bold text-gray-900">Quiz Hub</span>
                            )}
                        </div>
                        <button
                            onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
                            className="p-1 hover:bg-gray-100 rounded-lg transition-colors"
                        >
                            <ChevronRight className={`w-5 h-5 text-gray-600 transition-transform ${sidebarCollapsed ? '' : 'rotate-180'}`} />
                        </button>
                    </div>
                </div>

                {/* Lesson Planner Search Bar */}
                {!sidebarCollapsed && (
                    <div className="p-4 border-b border-gray-200">
                        <div className="relative">
                            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                            <input
                                type="text"
                                placeholder="Search planners..."
                                value={lessonPlannerSearch}
                                onChange={(e) => setLessonPlannerSearch(e.target.value)}
                                className="w-full pl-9 pr-3 py-2 text-sm border border-gray-200 rounded-lg focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500 transition-all"
                            />
                        </div>
                        {lessonPlannerSearch && (
                            <div className="mt-2 text-xs text-gray-500">
                                {filteredLessonPlanners.length} of {lessonPlanners.length} planners
                            </div>
                        )}
                    </div>
                )}

                {/* Lesson Planners List */}
                <div className="flex-1 overflow-hidden flex flex-col">
                    <div className={`px-4 pt-4 pb-2 ${sidebarCollapsed ? 'hidden' : ''}`}>
                        <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wider">Lesson Planners</h3>
                    </div>
                    
                    {/* Scrollable List Container */}
                    <div className="flex-1 overflow-y-auto px-4 pb-4">
                        {filteredLessonPlanners.length === 0 ? (
                            <div className={`${sidebarCollapsed ? 'hidden' : ''} text-center py-8`}>
                                <BookOpen className="w-12 h-12 text-gray-300 mx-auto mb-3" />
                                <p className="text-sm text-gray-500">
                                    {lessonPlannerSearch ? 'No planners found' : 'No lesson planners yet'}
                                </p>
                                {lessonPlannerSearch && (
                                    <button
                                        onClick={() => setLessonPlannerSearch('')}
                                        className="mt-2 text-xs text-indigo-600 hover:text-indigo-700"
                                    >
                                        Clear search
                                    </button>
                                )}
                            </div>
                        ) : (
                            <div className="space-y-2">
                                {filteredLessonPlanners.map((planner) => (
                                    <button
                                        key={planner.id}
                                        onClick={() => handleLessonPlannerSelect(planner)}
                                        className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg transition-all ${
                                            selectedLessonPlanner?.id === planner.id
                                                ? 'bg-indigo-50 text-indigo-700 border border-indigo-200'
                                                : 'hover:bg-gray-50 text-gray-700'
                                        }`}
                                        title={sidebarCollapsed ? planner.title : ''}
                                    >
                                        <BookOpen className="w-5 h-5 flex-shrink-0" />
                                        {!sidebarCollapsed && (
                                            <div className="text-left flex-1 min-w-0">
                                                <p className="font-medium text-sm line-clamp-1">{planner.title}</p>
                                                <p className="text-xs text-gray-500 line-clamp-1">
                                                    {planner.description || 'No description'}
                                                </p>
                                            </div>
                                        )}
                                        {!sidebarCollapsed && selectedLessonPlanner?.id === planner.id && (
                                            <div className="w-2 h-2 bg-indigo-600 rounded-full animate-pulse"></div>
                                        )}
                                    </button>
                                ))}
                            </div>
                        )}
                    </div>
                </div>

                {/* Sidebar Footer - Stats */}
                {!sidebarCollapsed && selectedLessonPlanner && (
                    <div className="p-4 border-t border-gray-200 bg-gray-50">
                        <div className="space-y-3">
                            <div className="text-xs text-gray-500 uppercase tracking-wider font-semibold mb-2">
                                Statistics
                            </div>
                            <div className="flex items-center justify-between text-sm">
                                <span className="text-gray-600">Total Quizzes</span>
                                <span className="font-bold text-gray-900">{stats.total}</span>
                            </div>
                            <div className="flex items-center justify-between text-sm">
                                <span className="text-gray-600">Questions</span>
                                <span className="font-bold text-gray-900">{stats.totalQuestions}</span>
                            </div>
                            <div className="flex items-center justify-between text-sm">
                                <span className="text-gray-600">Total Planners</span>
                                <span className="font-bold text-gray-900">{lessonPlanners.length}</span>
                            </div>
                        </div>
                    </div>
                )}
            </div>

            {/* Main Content */}
            <div className="flex-1 flex flex-col">
                {/* Header */}
                <div className="bg-white border-b border-gray-200 px-8 py-6">
                    <div className="flex items-center justify-between mb-6">
                        <div>
                            <h1 className="text-3xl font-bold text-gray-900">Quiz Management</h1>
                            <p className="text-gray-600 mt-1">
                                {selectedLessonPlanner ? `Managing quizzes for "${selectedLessonPlanner.title}"` : 'Select a lesson planner to get started'}
                            </p>
                        </div>
                        <div className="flex items-center gap-3">
                            {/* View Mode Toggle */}
                            <div className="bg-gray-100 rounded-lg p-1 flex">
                                <button
                                    onClick={() => setViewMode('grid')}
                                    className={`p-2 rounded ${viewMode === 'grid' ? 'bg-white shadow-sm' : ''}`}
                                >
                                    <Grid className="w-4 h-4" />
                                </button>
                                <button
                                    onClick={() => setViewMode('list')}
                                    className={`p-2 rounded ${viewMode === 'list' ? 'bg-white shadow-sm' : ''}`}
                                >
                                    <List className="w-4 h-4" />
                                </button>
                            </div>

                            {/* Action Buttons */}
                            <button
                                onClick={() => setShowAIModal(true)}
                                disabled={!selectedLessonPlanner || aiLoading}
                                className="px-5 py-2.5 bg-gradient-to-r from-purple-600 to-indigo-600 text-white rounded-lg hover:from-purple-700 hover:to-indigo-700 transition-all flex items-center gap-2 font-medium shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                {aiLoading ? (
                                    <>
                                        <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                        Generating...
                                    </>
                                ) : (
                                    <>
                                        <Sparkles className="w-4 h-4" />
                                        AI Generate
                                    </>
                                )}
                            </button>
                            <button
                                onClick={handleCreate}
                                disabled={!selectedLessonPlanner}
                                className="px-5 py-2.5 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-all flex items-center gap-2 font-medium shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <Plus className="w-4 h-4" />
                                Create Quiz
                            </button>
                        </div>
                    </div>

                    {/* Search and Filter Bar */}
                    <div className="flex items-center gap-4">
                        <div className="flex-1 relative">
                            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                            <input
                                type="text"
                                placeholder="Search quizzes..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-lg focus:border-indigo-500 focus:outline-none"
                            />
                        </div>
                        <button className="px-4 py-2.5 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors flex items-center gap-2">
                            <Filter className="w-4 h-4" />
                            Filters
                        </button>
                    </div>
                </div>

                {/* Stats Cards */}
                {selectedLessonPlanner && (
                    <div className="px-8 py-6">
                        <div className="grid grid-cols-4 gap-4">
                            <div className="bg-white rounded-xl border border-gray-200 p-4">
                                <div className="flex items-center justify-between mb-2">
                                    <span className="text-gray-600 text-sm">Total Quizzes</span>
                                    <FileQuestion className="w-5 h-5 text-indigo-600" />
                                </div>
                                <p className="text-2xl font-bold text-gray-900">{stats.total}</p>
                                <p className="text-xs text-gray-500 mt-1">All time</p>
                            </div>
                            <div className="bg-white rounded-xl border border-gray-200 p-4">
                                <div className="flex items-center justify-between mb-2">
                                    <span className="text-gray-600 text-sm">Recent Activity</span>
                                    <Clock className="w-5 h-5 text-purple-600" />
                                </div>
                                <p className="text-2xl font-bold text-gray-900">{stats.recentQuizzes}</p>
                                <p className="text-xs text-gray-500 mt-1">Last 7 days</p>
                            </div>
                        </div>
                    </div>
                )}

                {/* Notifications */}
                {(deleteSuccess || error || aiLoading) && (
                    <div className="px-8">
                        {aiLoading && (
                            <div className="bg-purple-50 border border-purple-200 rounded-lg p-4 flex items-center gap-3 mb-4">
                                <div className="w-8 h-8 border-3 border-purple-600 border-t-transparent rounded-full animate-spin"></div>
                                <div>
                                    <p className="text-purple-900 font-semibold">AI is crafting your quiz...</p>
                                    <p className="text-purple-700 text-sm">This usually takes 5-15 seconds</p>
                                </div>
                            </div>
                        )}
                        {deleteSuccess && (
                            <div className="bg-green-50 border border-green-200 rounded-lg p-4 flex items-center gap-3 mb-4">
                                <CheckCircle className="w-5 h-5 text-green-600" />
                                <p className="text-green-900">{deleteSuccess}</p>
                            </div>
                        )}
                        {error && (
                            <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-center gap-3 mb-4">
                                <AlertCircle className="w-5 h-5 text-red-600" />
                                <p className="text-red-900">{error}</p>
                            </div>
                        )}
                    </div>
                )}

                {/* Content Area */}
                <div className="flex-1 px-8 py-6 overflow-auto">
                    {!selectedLessonPlanner ? (
                        <div className="flex flex-col items-center justify-center h-full">
                            <div className="w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center mb-6">
                                <BookOpen className="w-12 h-12 text-gray-400" />
                            </div>
                            <h3 className="text-xl font-semibold text-gray-900 mb-2">Select a Lesson Planner</h3>
                            <p className="text-gray-600 text-center max-w-md">
                                Choose a lesson planner from the sidebar to view and manage its quizzes
                            </p>
                        </div>
                    ) : loading ? (
                        <div className="flex items-center justify-center h-full">
                            <div className="flex flex-col items-center gap-4">
                                <div className="w-12 h-12 border-4 border-indigo-600 border-t-transparent rounded-full animate-spin"></div>
                                <p className="text-gray-600">Loading quizzes...</p>
                            </div>
                        </div>
                    ) : filteredQuizzes.length === 0 ? (
                        <div className="flex flex-col items-center justify-center h-full">
                            <div className="w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center mb-6">
                                <FileQuestion className="w-12 h-12 text-gray-400" />
                            </div>
                            <h3 className="text-xl font-semibold text-gray-900 mb-2">
                                {searchTerm ? 'No quizzes found' : 'No quizzes yet'}
                            </h3>
                            <p className="text-gray-600 text-center max-w-md mb-6">
                                {searchTerm ? 'Try adjusting your search terms' : `Create your first quiz for "${selectedLessonPlanner.title}"`}
                            </p>
                            {!searchTerm && (
                                <div className="flex gap-3">
                                    <button
                                        onClick={() => setShowAIModal(true)}
                                        className="px-5 py-2.5 bg-gradient-to-r from-purple-600 to-indigo-600 text-white rounded-lg hover:from-purple-700 hover:to-indigo-700 transition-all flex items-center gap-2 font-medium"
                                    >
                                        <Sparkles className="w-4 h-4" />
                                        Generate with AI
                                    </button>
                                    <button
                                        onClick={handleCreate}
                                        className="px-5 py-2.5 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-all flex items-center gap-2 font-medium"
                                    >
                                        <Plus className="w-4 h-4" />
                                        Create Manually
                                    </button>
                                </div>
                            )}
                        </div>
                    ) : viewMode === 'grid' ? (
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                            {filteredQuizzes.map((quiz) => (
                                <div
                                    key={quiz.id}
                                    className="bg-white rounded-xl border border-gray-200 hover:border-indigo-300 hover:shadow-lg transition-all group cursor-pointer"
                                    onClick={() => setSelectedQuiz(quiz)}
                                >
                                    <div className="p-6">
                                        <div className="flex items-start justify-between mb-4">
                                            <div className="w-12 h-12 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg flex items-center justify-center">
                                                <FileQuestion className="w-6 h-6 text-white" />
                                            </div>
                                            <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                                                <button
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        handleEdit(quiz.id);
                                                    }}
                                                    className="p-1.5 hover:bg-gray-100 rounded-lg transition-colors"
                                                >
                                                    <Edit className="w-4 h-4 text-gray-600" />
                                                </button>
                                                <button
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        handleDelete(quiz.id);
                                                    }}
                                                    className="p-1.5 hover:bg-red-50 rounded-lg transition-colors"
                                                >
                                                    <Trash2 className="w-4 h-4 text-red-600" />
                                                </button>
                                            </div>
                                        </div>

                                        <h3 className="font-semibold text-gray-900 mb-2 line-clamp-2">
                                            {quiz.title}
                                        </h3>
                                        <p className="text-sm text-gray-600 mb-4 line-clamp-2">
                                            {quiz.description || 'No description'}
                                        </p>

                                        <div className="flex items-center justify-between text-sm">
                                            <div className="flex items-center gap-4">
                                                <div className="flex items-center gap-1 text-gray-500">
                                                    <FileQuestion className="w-4 h-4" />
                                                    <span>{quiz.totalQuestion || 0}</span>
                                                </div>
                                                {quiz.createdAt && (
                                                    <div className="flex items-center gap-1 text-gray-500">
                                                        <Calendar className="w-4 h-4" />
                                                        <span>{new Date(quiz.createdAt).toLocaleDateString()}</span>
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="h-1 bg-gradient-to-r from-indigo-500 to-purple-600 transform scale-x-0 group-hover:scale-x-100 transition-transform origin-left"></div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <div className="space-y-4">
                            {filteredQuizzes.map((quiz) => (
                                <div
                                    key={quiz.id}
                                    className="bg-white rounded-xl border border-gray-200 hover:border-indigo-300 hover:shadow-lg transition-all p-6 flex items-center justify-between group"
                                >
                                    <div className="flex items-center gap-4">
                                        <div className="w-12 h-12 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg flex items-center justify-center">
                                            <FileQuestion className="w-6 h-6 text-white" />
                                        </div>
                                        <div>
                                            <h3 className="font-semibold text-gray-900 mb-1">{quiz.title}</h3>
                                            <p className="text-sm text-gray-600">{quiz.description || 'No description'}</p>
                                            <div className="flex items-center gap-4 mt-2 text-sm text-gray-500">
                                                <span>{quiz.totalQuestion || 0} questions</span>
                                                {quiz.createdAt && (
                                                    <span>Created {new Date(quiz.createdAt).toLocaleDateString()}</span>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="flex items-center gap-2">
                                        <button
                                            onClick={() => handleEdit(quiz.id)}
                                            className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
                                        >
                                            <Edit className="w-5 h-5 text-gray-600" />
                                        </button>
                                        <button
                                            onClick={() => handleDelete(quiz.id)}
                                            className="p-2 hover:bg-red-50 rounded-lg transition-colors"
                                        >
                                            <Trash2 className="w-5 h-5 text-red-600" />
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>

            {/* AI Generation Modal */}
            {showAIModal && (
                <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                    <div className="bg-white rounded-2xl shadow-2xl max-w-lg w-full animate-in fade-in zoom-in duration-200">
                        {/* Modal Header */}
                        <div className="border-b border-gray-200 px-6 py-4 flex items-center justify-between">
                            <div className="flex items-center gap-3">
                                <div className="w-10 h-10 bg-gradient-to-br from-purple-500 to-indigo-600 rounded-lg flex items-center justify-center">
                                    <Zap className="w-5 h-5 text-white" />
                                </div>
                                <h3 className="text-xl font-semibold text-gray-900">AI Quiz Generator</h3>
                            </div>
                            <button
                                onClick={() => {
                                    setShowAIModal(false);
                                    setAiQuizTitle('');
                                    setAiQuizDescription('');
                                    setAiQuestionCount(5);
                                }}
                                className="p-1 hover:bg-gray-100 rounded-lg transition-colors"
                            >
                                <X className="w-5 h-5 text-gray-600" />
                            </button>
                        </div>

                        {/* Modal Body */}
                        <div className="px-6 py-4">
                            <div className="bg-gradient-to-r from-purple-50 to-indigo-50 border border-purple-200 rounded-lg p-4 mb-6">
                                <div className="flex items-start gap-3">
                                    <Sparkles className="w-5 h-5 text-purple-600 mt-0.5" />
                                    <div>
                                        <p className="text-purple-900 font-medium">AI will analyze your lesson</p>
                                        <p className="text-purple-700 text-sm mt-1">"{selectedLessonPlanner?.title}"</p>
                                        <p className="text-purple-600 text-xs mt-2">
                                            Our AI will generate relevant questions and answers automatically!
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div className="space-y-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Quiz Title (Optional)
                                    </label>
                                    <input
                                        type="text"
                                        value={aiQuizTitle}
                                        onChange={(e) => setAiQuizTitle(e.target.value)}
                                        placeholder={`AI Quiz for ${selectedLessonPlanner?.title}`}
                                        className="w-full border border-gray-200 rounded-lg px-4 py-2.5 focus:border-indigo-500 focus:outline-none"
                                    />
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Description (Optional)
                                    </label>
                                    <textarea
                                        value={aiQuizDescription}
                                        onChange={(e) => setAiQuizDescription(e.target.value)}
                                        placeholder="Auto-generated quiz from lesson content"
                                        rows={3}
                                        className="w-full border border-gray-200 rounded-lg px-4 py-2.5 focus:border-indigo-500 focus:outline-none resize-none"
                                    />
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Number of Questions
                                    </label>
                                    <div className="flex items-center gap-4">
                                        <input
                                            type="range"
                                            min="1"
                                            max="20"
                                            value={aiQuestionCount}
                                            onChange={(e) => setAiQuestionCount(parseInt(e.target.value))}
                                            className="flex-1"
                                        />
                                        <span className="w-12 text-center font-semibold text-lg text-indigo-600">
                                            {aiQuestionCount}
                                        </span>
                                    </div>
                                    <p className="text-xs text-gray-500 mt-2">Recommended: 5-10 questions for best results</p>
                                </div>
                            </div>
                        </div>

                        {/* Modal Footer */}
                        <div className="border-t border-gray-200 px-6 py-4 flex gap-3">
                            <button
                                onClick={() => {
                                    setShowAIModal(false);
                                    setAiQuizTitle('');
                                    setAiQuizDescription('');
                                    setAiQuestionCount(5);
                                }}
                                className="flex-1 px-4 py-2.5 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors font-medium"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleAIGenerate}
                                className="flex-1 px-4 py-2.5 bg-gradient-to-r from-purple-600 to-indigo-600 text-white rounded-lg hover:from-purple-700 hover:to-indigo-700 transition-all flex items-center justify-center gap-2 font-medium"
                            >
                                <Sparkles className="w-4 h-4" />
                                Generate Quiz
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default QuizManagement;