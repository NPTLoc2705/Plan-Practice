// src/app/pages/CreateQuiz.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Trash2, Save } from 'lucide-react';
import { QuizAPI } from '../components/APIService/QuizAPI';
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
            navigate('/quizmanagement');
        } catch (err) {
            console.error(err);
            setError(err.message || 'Failed to create quiz.');
        } finally {
            setLoading(false);
        }
    };


    return (
        <div className="p-6 bg-gray-50 min-h-screen">
            <div className="max-w-3xl mx-auto">
                <h1 className="text-2xl font-bold mb-4">Create New Quiz</h1>

                {error && (
                    <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">{error}</div>
                )}

                <form onSubmit={handleSubmit} className="space-y-6">
                    <div className="bg-white p-4 rounded shadow">
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Title
                        </label>
                        <input
                            type="text"
                            className="w-full border p-2 rounded"
                            value={quiz.title}
                            onChange={(e) =>
                                setQuiz({ ...quiz, title: e.target.value })
                            }
                            required
                        />
                        <label className="block text-sm font-medium text-gray-700 mt-3 mb-1">
                            Description
                        </label>
                        <textarea
                            className="w-full border p-2 rounded"
                            rows={3}
                            value={quiz.description}
                            onChange={(e) =>
                                setQuiz({ ...quiz, description: e.target.value })
                            }
                        />
                    </div>

                    <div className="bg-white p-4 rounded shadow">
                        <div className="flex justify-between items-center mb-3">
                            <h2 className="text-lg font-semibold">Questions</h2>
                            <button
                                type="button"
                                onClick={addQuestion}
                                className="text-blue-600 flex items-center gap-2"
                            >
                                <Plus /> Add Question
                            </button>
                        </div>

                        {quiz.questions.map((q, qIdx) => (
                            <div key={qIdx} className="border rounded p-3 mb-4">
                                <div className="flex justify-between items-center mb-2">
                                    <input
                                        type="text"
                                        value={q.text}
                                        onChange={(e) =>
                                            updateQuestionText(qIdx, e.target.value)
                                        }
                                        placeholder={`Question ${qIdx + 1}`}
                                        className="w-full border-b p-1 mr-3"
                                    />
                                    <button
                                        type="button"
                                        onClick={() => removeQuestion(qIdx)}
                                        className="text-red-600"
                                    >
                                        <Trash2 />
                                    </button>
                                </div>

                                <div className="ml-2">
                                    <h3 className="text-sm font-medium mb-2">
                                        Answers
                                    </h3>
                                    {q.answers.map((a, aIdx) => (
                                        <div
                                            key={aIdx}
                                            className="flex items-center gap-2 mb-2"
                                        >
                                            <input
                                                type="text"
                                                className="flex-1 border p-2 rounded"
                                                value={a.text}
                                                onChange={(e) =>
                                                    updateAnswerText(
                                                        qIdx,
                                                        aIdx,
                                                        e.target.value
                                                    )
                                                }
                                                placeholder={`Answer ${aIdx + 1}`}
                                            />
                                            <label className="flex items-center gap-1">
                                                <input
                                                    type="radio"
                                                    name={`correct-${qIdx}`}
                                                    checked={a.isCorrect}
                                                    onChange={() =>
                                                        setCorrectAnswer(qIdx, aIdx)
                                                    }
                                                />
                                                <span className="text-sm">
                                                    Correct
                                                </span>
                                            </label>
                                            <button
                                                type="button"
                                                onClick={() =>
                                                    removeAnswer(qIdx, aIdx)
                                                }
                                                className="text-red-500 ml-2"
                                            >
                                                <Trash2 />
                                            </button>
                                        </div>
                                    ))}

                                    <button
                                        type="button"
                                        onClick={() => addAnswer(qIdx)}
                                        className="text-blue-600 text-sm"
                                    >
                                        + Add Answer
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>

                    <div className="flex gap-3">
                        <button
                            type="submit"
                            disabled={loading}
                            className="bg-green-600 text-white px-4 py-2 rounded flex items-center gap-2"
                        >
                            {loading ? 'Saving...' : (<><Save /> Save Quiz</>)}
                        </button>
                        <button
                            type="button"
                            onClick={() => navigate(-1)}
                            className="px-4 py-2 border rounded"
                        >
                            Cancel
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
