import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import QuizAPI from '../components/APIService/StudentQuizAPI'
import styles from './QuizList.module.css'
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

if (loading) return <div className={styles.loading}>Loading quizzes...</div>
if (error) return <div className={styles.error}>{error}</div>
   return (
    <div className={styles.quizListContainer}>
      <div className={styles.quizHeader}>
        <h2>Available Quizzes</h2>
        <p>Select a quiz to begin</p>
      </div>
      
      {quizzes.length === 0 ? (
        <p>No quizzes available at the moment.</p>
      ) : (
        <div className={styles.quizGrid}>
          {quizzes.map((quiz) => (
            <div key={quiz.id} className={styles.quizCard}>
              <h3>{quiz.title}</h3>
              <p>{quiz.description}</p>
              <div className={styles.quizInfo}>
                <span>Questions: {quiz.totalQuestions}</span>
                {quiz.hasAttempted && (
                  <span className={styles.attemptedBadge}>Attempted</span>
                )}
              </div>
              <button 
                onClick={() => handleStartQuiz(quiz.id)}
                className={styles.takeQuizButton}
                disabled={quiz.hasAttempted}
              >
                {quiz.hasAttempted ? 'View Result' : 'Take Quiz'}
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default QuizList