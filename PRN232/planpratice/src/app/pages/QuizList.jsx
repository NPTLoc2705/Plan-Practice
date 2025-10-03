import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import QuizAPI from '../components/APIService/StudentQuizAPI'

function QuizList() {
  const [quizzes, setQuizzes] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const navigate = useNavigate()

  useEffect(() => {
    fetchQuizzes()
  }, [])

  const fetchQuizzes = async () => {
    try {
      setLoading(true)
      const response = await QuizAPI.getAvailableQuizzes()
      if (response.success) {
        setQuizzes(response.data || [])
      }
    } catch (err) {
      setError(err.message || 'Failed to load quizzes')
      console.error('Error:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleStartQuiz = async (quizId) => {
    try {
      const checkResponse = await QuizAPI.checkQuizAttempt(quizId)
      if (checkResponse.data?.hasAttempted) {
        if (!window.confirm('You have already taken this quiz. View history instead?')) {
          return
        }
        navigate('/history')
        return
      }
      navigate(`/quiz/${quizId}`)
    } catch (err) {
      setError(err.message || 'Failed to check quiz status')
    }
  }

  if (loading) return <div className="loading">Loading quizzes...</div>
  if (error) return <div className="error">{error}</div>

  return (
    <div className="quiz-list-container">
      <h2>Available Quizzes</h2>
      {quizzes.length === 0 ? (
        <p>No quizzes available at the moment.</p>
      ) : (
        <div className="quiz-grid">
          {quizzes.map((quiz) => (
            <div key={quiz.id} className="quiz-card">
              <h3>{quiz.title}</h3>
              <p>{quiz.description}</p>
              <div className="quiz-info">
                <span>Questions: {quiz.totalQuestions}</span>
                {quiz.hasAttempted && (
                  <span className="attempted-badge">Attempted</span>
                )}
              </div>
              {quiz.lastResult && (
                <div className="last-result">
                  Last Score: {quiz.lastResult.score}/{quiz.lastResult.totalQuestions} 
                  ({quiz.lastResult.percentage}%)
                </div>
              )}
              <button 
                onClick={() => handleStartQuiz(quiz.id)}
                className="btn-primary"
              >
                {quiz.hasAttempted ? 'View Details' : 'Start Quiz'}
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default QuizList