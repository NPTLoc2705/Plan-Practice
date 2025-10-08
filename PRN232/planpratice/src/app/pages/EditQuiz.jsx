import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { QuizAPI } from '../components/APIService/QuizAPI';
import { Save, Plus, X, Edit3, CheckCircle } from 'lucide-react';
import './output.css'; // Ensure Tailwind CSS is imported
const EditQuiz = () => {
    const { quizId } = useParams();
    const navigate = useNavigate();

    const [quiz, setQuiz] = useState({ title: '', description: '' });
    const [questions, setQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState('');

    // 🧠 Modal states
    const [showQuestionModal, setShowQuestionModal] = useState(false);
    const [showAnswerModal, setShowAnswerModal] = useState(false);
    const [newQuestionText, setNewQuestionText] = useState('');
    const [newAnswerText, setNewAnswerText] = useState('');
    const [isCorrect, setIsCorrect] = useState(false);
    const [currentQuestionId, setCurrentQuestionId] = useState(null);

    // 🧠 Load quiz info + questions + answers
    useEffect(() => {
        const fetchData = async () => {
            try {
                const quizData = await QuizAPI.getQuizById(quizId);
                setQuiz({
                    title: quizData.title || '',
                    description: quizData.description || '',
                });

                const questionsData = await QuizAPI.getQuestionsByQuizId(quizId);
                const questionsWithAnswers = await Promise.all(
                    questionsData.map(async (q) => {
                        const answers = await QuizAPI.getAnswersByQuestionId(q.id);
                        return { ...q, answers };
                    })
                );

                setQuestions(questionsWithAnswers);
            } catch (err) {
                console.error(err);
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, [quizId]);

    // 🧩 Handle quiz info changes
    const handleChange = (e) => {
        const { name, value } = e.target;
        setQuiz((prev) => ({ ...prev, [name]: value }));
    };

    // 🧠 Update question text
    const handleQuestionChange = (index, value) => {
        const updated = [...questions];
        updated[index].content = value;
        setQuestions(updated);
    };

    // 🧩 Update answer text
    const handleAnswerChange = (qIdx, aIdx, value) => {
        const updated = [...questions];
        updated[qIdx].answers[aIdx].content = value;
        setQuestions(updated);
    };

    // ✅ Set correct answer
    const setCorrectAnswer = (qIdx, aIdx) => {
        const updated = [...questions];
        updated[qIdx].answers = updated[qIdx].answers.map((a, i) => ({
            ...a,
            isCorrect: i === aIdx,
        }));
        setQuestions(updated);
    };

    // 🟢 Open modals
    const openQuestionModal = () => {
        setNewQuestionText('');
        setShowQuestionModal(true);
    };

    const openAnswerModal = (questionId) => {
        setNewAnswerText('');
        setIsCorrect(false);
        setCurrentQuestionId(questionId);
        setShowAnswerModal(true);
    };

    // 💾 Save question
    const handleSaveQuestion = async () => {
        if (!newQuestionText.trim()) return alert('Please enter a question!');
        try {
            const newQuestion = await QuizAPI.createQuestion({
                content: newQuestionText,
                quizId: quizId,
            });
            setQuestions([...questions, { ...newQuestion, answers: [] }]);
            setShowQuestionModal(false);
        } catch (err) {
            setError(err.message);
        }
    };

    // 💾 Save answer
    const handleSaveAnswer = async () => {
        if (!newAnswerText.trim()) return alert('Please enter an answer!');
        try {
            const newAnswer = await QuizAPI.createAnswer({
                content: newAnswerText,
                isCorrect,
                questionId: currentQuestionId,
            });

            setQuestions((prev) =>
                prev.map((q) =>
                    q.id === currentQuestionId
                        ? { ...q, answers: [...q.answers, newAnswer] }
                        : q
                )
            );

            setShowAnswerModal(false);
        } catch (err) {
            setError(err.message);
        }
    };

    // 💾 Save all changes
    const handleSaveAll = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');
        try {
            await QuizAPI.updateQuiz(quizId, quiz);

            for (const q of questions) {
                await QuizAPI.updateQuestion(q.id, {
                    id: q.id,
                    content: q.content,
                    quizId: quizId,
                });

                for (const a of q.answers) {
                    await QuizAPI.updateAnswer(a.id, {
                        id: a.id,
                        content: a.content,
                        isCorrect: a.isCorrect,
                        questionId: q.id,
                    });
                }
            }

            setSuccess('Quiz, questions, and answers updated successfully!');
            setTimeout(() => navigate('/quizmanagement'), 1500);
        } catch (err) {
            console.error(err);
            setError(err.message);
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 flex items-center justify-center">
                <div className="text-lg text-teal-600 font-medium">Loading quiz data...</div>
            </div>
        );
    }

    if (error && !questions.length) {
        return (
            <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 flex items-center justify-center">
                <div className="bg-white rounded-2xl shadow-xl p-8 max-w-md">
                    <div className="text-red-500 text-lg font-medium">Error: {error}</div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 py-8 px-4">
            <div className="max-w-6xl mx-auto">
                {/* Header Card */}
                <div className="bg-white rounded-2xl shadow-lg p-8 mb-6">
                    <div className="flex items-center gap-4 mb-6">
                        <div className="w-12 h-12 bg-gradient-to-br from-teal-400 to-emerald-500 rounded-xl flex items-center justify-center">
                            <Edit3 className="w-6 h-6 text-white" />
                        </div>
                        <h1 className="text-3xl font-bold text-gray-800">Edit Quiz</h1>
                    </div>

                    <form onSubmit={handleSaveAll} className="space-y-6">
                        {/* Quiz Info Section */}
                        <div className="grid md:grid-cols-2 gap-6">
                            <div>
                                <label className="block text-sm font-semibold text-gray-700 mb-2">
                                    Quiz Title
                                </label>
                                <input
                                    type="text"
                                    name="title"
                                    value={quiz.title}
                                    onChange={handleChange}
                                    className="w-full border-2 border-gray-200 rounded-xl px-4 py-3 focus:border-teal-500 focus:outline-none transition-colors"
                                    placeholder="Enter quiz title..."
                                    required
                                />
                            </div>

                            <div>
                                <label className="block text-sm font-semibold text-gray-700 mb-2">
                                    Description
                                </label>
                                <textarea
                                    name="description"
                                    value={quiz.description}
                                    onChange={handleChange}
                                    rows="3"
                                    className="w-full border-2 border-gray-200 rounded-xl px-4 py-3 focus:border-teal-500 focus:outline-none transition-colors resize-none"
                                    placeholder="Enter quiz description..."
                                    required
                                />
                            </div>
                        </div>

                        {/* Action Buttons - Top */}
                        <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                            <button
                                type="button"
                                onClick={() => navigate('/teacher')}
                                className="px-6 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-all font-medium"
                            >
                                Cancel
                            </button>

                            <button
                                type="submit"
                                className="px-6 py-3 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all flex items-center gap-2 font-medium shadow-lg shadow-teal-200"
                            >
                                <Save className="w-5 h-5" /> Save All Changes
                            </button>
                        </div>

                        {/* Success/Error Messages */}
                        {error && (
                            <div className="bg-red-50 border-l-4 border-red-500 rounded-lg p-4">
                                <p className="text-red-700 font-medium">{error}</p>
                            </div>
                        )}
                        {success && (
                            <div className="bg-emerald-50 border-l-4 border-emerald-500 rounded-lg p-4 flex items-center gap-2">
                                <CheckCircle className="w-5 h-5 text-emerald-600" />
                                <p className="text-emerald-700 font-medium">{success}</p>
                            </div>
                        )}
                    </form>
                </div>

                {/* Questions Section */}
                <div className="bg-white rounded-2xl shadow-lg p-8">
                    <div className="flex justify-between items-center mb-6">
                        <h2 className="text-2xl font-bold text-gray-800">Questions</h2>
                        <button
                            type="button"
                            onClick={openQuestionModal}
                            className="px-5 py-3 bg-gradient-to-r from-emerald-500 to-teal-500 text-white rounded-xl hover:from-emerald-600 hover:to-teal-600 transition-all flex items-center gap-2 font-medium shadow-lg shadow-emerald-200"
                        >
                            <Plus className="w-5 h-5" /> Add Question
                        </button>
                    </div>

                    <div className="space-y-6">
                        {questions.map((q, qIdx) => (
                            <div
                                key={q.id}
                                className="border-2 border-gray-200 rounded-2xl p-6 hover:border-teal-300 transition-all bg-gradient-to-br from-gray-50 to-white"
                            >
                                <div className="flex items-start gap-3 mb-4">
                                    <div className="w-8 h-8 bg-teal-500 text-white rounded-lg flex items-center justify-center font-bold text-sm flex-shrink-0 mt-1">
                                        {qIdx + 1}
                                    </div>
                                    <input
                                        type="text"
                                        value={q.content}
                                        onChange={(e) => handleQuestionChange(qIdx, e.target.value)}
                                        className="flex-1 border-b-2 border-gray-300 p-2 font-semibold text-gray-800 bg-transparent focus:border-teal-500 focus:outline-none transition-colors"
                                        placeholder="Enter question text..."
                                    />
                                </div>

                                <div className="ml-11 space-y-3">
                                    {q.answers.map((a, aIdx) => (
                                        <div
                                            key={a.id}
                                            className={`flex items-center gap-3 p-3 rounded-xl transition-all ${a.isCorrect
                                                    ? 'bg-emerald-50 border-2 border-emerald-300'
                                                    : 'bg-white border-2 border-gray-200 hover:border-gray-300'
                                                }`}
                                        >
                                            <input
                                                type="text"
                                                value={a.content}
                                                onChange={(e) =>
                                                    handleAnswerChange(qIdx, aIdx, e.target.value)
                                                }
                                                className="flex-1 px-3 py-2 bg-transparent focus:outline-none"
                                                placeholder="Enter answer text..."
                                            />
                                            <label className="flex items-center gap-2 cursor-pointer group">
                                                <input
                                                    type="radio"
                                                    name={`correct-${qIdx}`}
                                                    checked={a.isCorrect}
                                                    onChange={() => setCorrectAnswer(qIdx, aIdx)}
                                                    className="w-5 h-5 text-emerald-500 focus:ring-emerald-500"
                                                />
                                                <span className={`text-sm font-medium ${a.isCorrect ? 'text-emerald-600' : 'text-gray-600'}`}>
                                                    Correct
                                                </span>
                                            </label>
                                        </div>
                                    ))}
                                </div>

                                <button
                                    type="button"
                                    onClick={() => openAnswerModal(q.id)}
                                    className="ml-11 mt-4 px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-all flex items-center gap-2 text-sm font-medium"
                                >
                                    <Plus className="w-4 h-4" /> Add Answer
                                </button>
                            </div>
                        ))}

                        {questions.length === 0 && (
                            <div className="text-center py-12 text-gray-400">
                                <p className="text-lg">No questions yet. Click "Add Question" to get started!</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* ===== MODALS ===== */}

            {/* Question Modal */}
            {showQuestionModal && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
                    <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg transform transition-all">
                        <div className="flex items-center justify-between p-6 border-b border-gray-200">
                            <h2 className="text-xl font-bold text-gray-800">Add New Question</h2>
                            <button
                                onClick={() => setShowQuestionModal(false)}
                                className="w-8 h-8 flex items-center justify-center rounded-lg hover:bg-gray-100 transition-colors"
                            >
                                <X className="w-5 h-5 text-gray-500" />
                            </button>
                        </div>

                        <div className="p-6">
                            <textarea
                                rows="4"
                                value={newQuestionText}
                                onChange={(e) => setNewQuestionText(e.target.value)}
                                placeholder="Enter your question here..."
                                className="w-full border-2 border-gray-200 rounded-xl p-4 focus:border-teal-500 focus:outline-none transition-colors resize-none"
                            />
                        </div>

                        <div className="flex justify-end gap-3 p-6 border-t border-gray-200">
                            <button
                                onClick={() => setShowQuestionModal(false)}
                                className="px-5 py-2.5 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-all font-medium"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSaveQuestion}
                                className="px-5 py-2.5 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all font-medium"
                            >
                                Save Question
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Answer Modal */}
            {showAnswerModal && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
                    <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg transform transition-all">
                        <div className="flex items-center justify-between p-6 border-b border-gray-200">
                            <h2 className="text-xl font-bold text-gray-800">Add New Answer</h2>
                            <button
                                onClick={() => setShowAnswerModal(false)}
                                className="w-8 h-8 flex items-center justify-center rounded-lg hover:bg-gray-100 transition-colors"
                            >
                                <X className="w-5 h-5 text-gray-500" />
                            </button>
                        </div>

                        <div className="p-6 space-y-4">
                            <textarea
                                rows="3"
                                value={newAnswerText}
                                onChange={(e) => setNewAnswerText(e.target.value)}
                                placeholder="Enter answer text..."
                                className="w-full border-2 border-gray-200 rounded-xl p-4 focus:border-teal-500 focus:outline-none transition-colors resize-none"
                            />

                            <label className="flex items-center gap-3 p-4 bg-emerald-50 rounded-xl cursor-pointer hover:bg-emerald-100 transition-colors">
                                <input
                                    type="checkbox"
                                    checked={isCorrect}
                                    onChange={(e) => setIsCorrect(e.target.checked)}
                                    className="w-5 h-5 text-emerald-500 focus:ring-emerald-500 rounded"
                                />
                                <span className="font-medium text-gray-700">Mark as correct answer</span>
                            </label>
                        </div>

                        <div className="flex justify-end gap-3 p-6 border-t border-gray-200">
                            <button
                                onClick={() => setShowAnswerModal(false)}
                                className="px-5 py-2.5 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-all font-medium"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSaveAnswer}
                                className="px-5 py-2.5 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all font-medium"
                            >
                                Save Answer
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default EditQuiz;