﻿// src/app/pages/CreateQuiz.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Trash2, Save, FileQuestion, CheckCircle, AlertCircle } from 'lucide-react';
import { QuizAPI } from '../components/APIService/QuizAPI';
import './output.css'; // Ensure Tailwind CSS is imported
export default function CreateQuiz() {
    const navigate = useNavigate();
    const [quiz, setQuiz] = useState({
        title: '',
        description: '',
        questions: [],
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const addQuestion = () => {
        setQuiz((p) => ({
            ...p,
            questions: [...p.questions, { text: '', answers: [{ text: '', isCorrect: false }] }],
        }));
    };

    const removeQuestion = (qIdx) => {
        setQuiz((p) => ({ ...p, questions: p.questions.filter((_, i) => i !== qIdx) }));
    };

    const updateQuestionText = (qIdx, value) => {
        setQuiz((p) => {
            const q = [...p.questions];
            q[qIdx].text = value;
            return { ...p, questions: q };
        });
    };

    const addAnswer = (qIdx) => {
        setQuiz((p) => {
            const q = [...p.questions];
            q[qIdx].answers.push({ text: '', isCorrect: false });
            return { ...p, questions: q };
        });
    };

    const removeAnswer = (qIdx, aIdx) => {
        setQuiz((p) => {
            const q = [...p.questions];
            q[qIdx].answers = q[qIdx].answers.filter((_, i) => i !== aIdx);
            return { ...p, questions: q };
        });
    };

    const updateAnswerText = (qIdx, aIdx, value) => {
        setQuiz((p) => {
            const q = [...p.questions];
            q[qIdx].answers[aIdx].text = value;
            return { ...p, questions: q };
        });
    };

    const setCorrectAnswer = (qIdx, aIdx) => {
        setQuiz((p) => {
            const q = [...p.questions];
            q[qIdx].answers = q[qIdx].answers.map((a, i) => ({ ...a, isCorrect: i === aIdx }));
            return { ...p, questions: q };
        });
    };

    const validate = () => {
        if (!quiz.title.trim()) return 'Please enter a quiz title.';
        if (quiz.questions.length === 0) return 'Add at least one question.';
        for (const [i, q] of quiz.questions.entries()) {
            if (!q.text.trim()) return `Question ${i + 1} is empty.`;
            if (!q.answers || q.answers.length === 0)
                return `Question ${i + 1} must have at least one answer.`;
            if (!q.answers.some((a) => a.isCorrect))
                return `Question ${i + 1} must have a correct answer.`;
        }
        return null;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        const v = validate();
        if (v) {
            setError(v);
            return;
        }

        setLoading(true);
        try {
            // 1️⃣ Create Quiz first
            const quizRes = await QuizAPI.createQuiz({
                title: quiz.title,
                description: quiz.description,
            });
            const quizId = quizRes.id; // assuming backend returns quiz id

            // 2️⃣ Create each Question and its Answers
            for (const q of quiz.questions) {
                const questionRes = await QuizAPI.createQuestion({
                    content: q.text,
                    quizId,
                });

                // 3️⃣ Create answers for this question
                for (const a of q.answers) {
                    await QuizAPI.createAnswer({
                        content: a.text,
                        isCorrect: a.isCorrect,
                        questionId: questionRes.id,
                    });
                }
            }

            alert('Quiz created successfully!');
            navigate('/teacher/quiz');
        } catch (err) {
            console.error(err);
            setError(err.message || 'Failed to create quiz.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 py-8 px-4">
            <div className="max-w-6xl mx-auto">
                {/* Header Card */}
                <div className="bg-white rounded-2xl shadow-lg p-8 mb-6">
                    <div className="flex items-center gap-4 mb-6">
                        <div className="w-12 h-12 bg-gradient-to-br from-teal-400 to-emerald-500 rounded-xl flex items-center justify-center">
                            <FileQuestion className="w-6 h-6 text-white" />
                        </div>
                        <div>
                            <h1 className="text-3xl font-bold text-gray-800">Create New Quiz</h1>
                            <p className="text-gray-500 text-sm mt-1">Build engaging quizzes with multiple questions and answers</p>
                        </div>
                    </div>

                    {error && (
                        <div className="bg-red-50 border-l-4 border-red-500 rounded-lg p-4 flex items-start gap-3">
                            <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
                            <p className="text-red-700 font-medium">{error}</p>
                        </div>
                    )}
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                    {/* Quiz Info Section */}
                    <div className="bg-white rounded-2xl shadow-lg p-8">
                        <h2 className="text-xl font-bold text-gray-800 mb-6">Quiz Information</h2>

                        <div className="space-y-6">
                            <div>
                                <label className="block text-sm font-semibold text-gray-700 mb-2">
                                    Quiz Title <span className="text-red-500">*</span>
                                </label>
                                <input
                                    type="text"
                                    className="w-full border-2 border-gray-200 rounded-xl px-4 py-3 focus:border-teal-500 focus:outline-none transition-colors"
                                    placeholder="Enter an engaging quiz title..."
                                    value={quiz.title}
                                    onChange={(e) => setQuiz({ ...quiz, title: e.target.value })}
                                    required
                                />
                            </div>

                            <div>
                                <label className="block text-sm font-semibold text-gray-700 mb-2">
                                    Description
                                </label>
                                <textarea
                                    className="w-full border-2 border-gray-200 rounded-xl px-4 py-3 focus:border-teal-500 focus:outline-none transition-colors resize-none"
                                    rows={4}
                                    placeholder="Describe what this quiz is about..."
                                    value={quiz.description}
                                    onChange={(e) => setQuiz({ ...quiz, description: e.target.value })}
                                />
                            </div>
                        </div>
                    </div>

                    {/* Questions Section */}
                    <div className="bg-white rounded-2xl shadow-lg p-8">
                        <div className="flex justify-between items-center mb-6">
                            <div>
                                <h2 className="text-xl font-bold text-gray-800">Questions</h2>
                                <p className="text-sm text-gray-500 mt-1">
                                    {quiz.questions.length} question{quiz.questions.length !== 1 ? 's' : ''} added
                                </p>
                            </div>
                            <button
                                type="button"
                                onClick={addQuestion}
                                className="px-5 py-3 bg-gradient-to-r from-emerald-500 to-teal-500 text-white rounded-xl hover:from-emerald-600 hover:to-teal-600 transition-all flex items-center gap-2 font-medium shadow-lg shadow-emerald-200"
                            >
                                <Plus className="w-5 h-5" /> Add Question
                            </button>
                        </div>

                        {quiz.questions.length === 0 ? (
                            <div className="text-center py-12 text-gray-400">
                                <FileQuestion className="w-16 h-16 mx-auto mb-4 opacity-50" />
                                <p className="text-lg font-medium">No questions yet</p>
                                <p className="text-sm mt-2">Click "Add Question" to get started!</p>
                            </div>
                        ) : (
                            <div className="space-y-6">
                                {quiz.questions.map((q, qIdx) => (
                                    <div
                                        key={qIdx}
                                        className="border-2 border-gray-200 rounded-2xl p-6 hover:border-teal-300 transition-all bg-gradient-to-br from-gray-50 to-white"
                                    >
                                        {/* Question Header */}
                                        <div className="flex items-start gap-3 mb-4">
                                            <div className="w-8 h-8 bg-teal-500 text-white rounded-lg flex items-center justify-center font-bold text-sm flex-shrink-0 mt-1">
                                                {qIdx + 1}
                                            </div>
                                            <input
                                                type="text"
                                                value={q.text}
                                                onChange={(e) => updateQuestionText(qIdx, e.target.value)}
                                                placeholder={`Enter question ${qIdx + 1}...`}
                                                className="flex-1 border-b-2 border-gray-300 p-2 font-semibold text-gray-800 bg-transparent focus:border-teal-500 focus:outline-none transition-colors"
                                            />
                                            <button
                                                type="button"
                                                onClick={() => removeQuestion(qIdx)}
                                                className="w-9 h-9 flex items-center justify-center rounded-lg text-red-500 hover:bg-red-50 transition-colors flex-shrink-0"
                                                title="Delete question"
                                            >
                                                <Trash2 className="w-5 h-5" />
                                            </button>
                                        </div>

                                        {/* Answers Section */}
                                        <div className="ml-11">
                                            <div className="flex items-center justify-between mb-3">
                                                <h3 className="text-sm font-semibold text-gray-700">
                                                    Answers
                                                </h3>
                                                <button
                                                    type="button"
                                                    onClick={() => addAnswer(qIdx)}
                                                    className="px-3 py-1.5 text-sm bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-all flex items-center gap-1.5 font-medium"
                                                >
                                                    <Plus className="w-4 h-4" /> Add Answer
                                                </button>
                                            </div>

                                            <div className="space-y-3">
                                                {q.answers.map((a, aIdx) => (
                                                    <div
                                                        key={aIdx}
                                                        className={`flex items-center gap-3 p-3 rounded-xl transition-all ${a.isCorrect
                                                                ? 'bg-emerald-50 border-2 border-emerald-300'
                                                                : 'bg-white border-2 border-gray-200 hover:border-gray-300'
                                                            }`}
                                                    >
                                                        <input
                                                            type="text"
                                                            className="flex-1 px-3 py-2 bg-transparent focus:outline-none"
                                                            value={a.text}
                                                            onChange={(e) =>
                                                                updateAnswerText(qIdx, aIdx, e.target.value)
                                                            }
                                                            placeholder={`Answer ${aIdx + 1}`}
                                                        />
                                                        <label className="flex items-center gap-2 cursor-pointer group">
                                                            <input
                                                                type="radio"
                                                                name={`correct-${qIdx}`}
                                                                checked={a.isCorrect}
                                                                onChange={() => setCorrectAnswer(qIdx, aIdx)}
                                                                className="w-5 h-5 text-emerald-500 focus:ring-emerald-500"
                                                            />
                                                            <span className={`text-sm font-medium whitespace-nowrap ${a.isCorrect ? 'text-emerald-600' : 'text-gray-600'
                                                                }`}>
                                                                {a.isCorrect && <CheckCircle className="w-4 h-4 inline mr-1" />}
                                                                Correct
                                                            </span>
                                                        </label>
                                                        <button
                                                            type="button"
                                                            onClick={() => removeAnswer(qIdx, aIdx)}
                                                            className="w-8 h-8 flex items-center justify-center rounded-lg text-red-500 hover:bg-red-50 transition-colors"
                                                            title="Delete answer"
                                                        >
                                                            <Trash2 className="w-4 h-4" />
                                                        </button>
                                                    </div>
                                                ))}
                                            </div>

                                            {q.answers.length === 0 && (
                                                <div className="text-center py-6 text-gray-400 text-sm">
                                                    No answers yet. Add at least one answer.
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                    {/* Action Buttons */}
                    <div className="bg-white rounded-2xl shadow-lg p-6">
                        <div className="flex justify-end gap-3">
                            <button
                                type="button"
                                onClick={() => navigate(-1)}
                                className="px-6 py-3 bg-gray-100 text-gray-700 rounded-xl hover:bg-gray-200 transition-all font-medium"
                            >
                                Cancel
                            </button>
                            <button
                                type="submit"
                                disabled={loading}
                                className="px-6 py-3 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl hover:from-teal-600 hover:to-emerald-600 transition-all flex items-center gap-2 font-medium shadow-lg shadow-teal-200 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                {loading ? (
                                    <>
                                        <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                        Saving...
                                    </>
                                ) : (
                                    <>
                                        <Save className="w-5 h-5" /> Save Quiz
                                    </>
                                )}
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}