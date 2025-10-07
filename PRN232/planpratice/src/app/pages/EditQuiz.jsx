import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { QuizAPI } from '../components/APIService/QuizAPI';

const EditQuiz = () => {
    const { quizId } = useParams();
    const navigate = useNavigate();

    const [quiz, setQuiz] = useState({ title: '', description: '' });
    const [questions, setQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState('');

    // 🧠 Load quiz info + questions + answers
    useEffect(() => {
        const fetchData = async () => {
            try {
                // Get quiz basic info
                const quizData = await QuizAPI.getQuizById(quizId);
                setQuiz({
                    title: quizData.title || '',
                    description: quizData.description || '',
                });

                // Get quiz questions
                const questionsData = await QuizAPI.getQuestionsByQuizId(quizId);

                // For each question, fetch its answers
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

    // 📝 Handle quiz info update
    const handleChange = (e) => {
        const { name, value } = e.target;
        setQuiz((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        try {
            await QuizAPI.updateQuiz(quizId, quiz);
            setSuccess('Quiz updated successfully!');
            setTimeout(() => navigate('/quizmanagement'), 1000);
        } catch (err) {
            setError(err.message);
        }
    };

    if (loading) return <div className="p-6 text-gray-500">Loading quiz data...</div>;
    if (error) return <div className="p-6 text-red-500">Error: {error}</div>;

    return (
        <div className="p-8 max-w-4xl mx-auto bg-white shadow-lg rounded-xl">
            <h1 className="text-2xl font-semibold text-gray-800 mb-6">Edit Quiz</h1>

            {/* 🔹 QUIZ INFO */}
            <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                    <label className="block text-gray-700 font-medium mb-2">Title</label>
                    <input
                        type="text"
                        name="title"
                        value={quiz.title}
                        onChange={handleChange}
                        className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500 outline-none"
                        required
                    />
                </div>

                <div>
                    <label className="block text-gray-700 font-medium mb-2">Description</label>
                    <textarea
                        name="description"
                        value={quiz.description}
                        onChange={handleChange}
                        rows="4"
                        className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500 outline-none"
                        required
                    />
                </div>

                {error && <p className="text-red-600 text-sm">{error}</p>}
                {success && <p className="text-green-600 text-sm">{success}</p>}

                <div className="flex justify-between mt-6">
                    <button
                        type="button"
                        onClick={() => navigate('/teacher')}
                        className="px-4 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition"
                    >
                        Cancel
                    </button>

                    <button
                        type="submit"
                        className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition"
                    >
                        Save Changes
                    </button>
                </div>
            </form>

            {/* 🔹 QUESTIONS SECTION */}
            <div className="mt-10">
                <h2 className="text-xl font-semibold text-gray-800 mb-4">Questions</h2>

                {questions.length === 0 ? (
                    <p className="text-gray-500">No questions found for this quiz.</p>
                ) : (
                    <div className="space-y-6">
                        {questions.map((question, index) => (
                            <div
                                key={question.id}
                                className="border border-gray-200 rounded-lg p-4 shadow-sm"
                            >
                                <h3 className="font-medium text-gray-800 mb-2">
                                    {index + 1}. {question.content}
                                </h3>
                                <ul className="ml-5 list-disc text-gray-700">
                                    {question.answers.map((ans) => (
                                        <li
                                            key={ans.id}
                                            className={
                                                ans.isCorrect
                                                    ? 'text-green-600 font-semibold'
                                                    : 'text-gray-600'
                                            }
                                        >
                                            {ans.content}
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default EditQuiz;
