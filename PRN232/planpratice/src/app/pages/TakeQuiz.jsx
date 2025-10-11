import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import QuizAPI from '../components/APIService/StudentQuizAPI'
import styles from './TakeQuiz.module.css'
function TakeQuiz() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [quiz, setQuiz] = useState(null)
  const [currentQuestion, setCurrentQuestion] = useState(0)
  const [answers, setAnswers] = useState({})
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchQuiz()
  }, [id])

  const fetchQuiz = async () => {
    try {
      setLoading(true)
      const response = await QuizAPI.getQuizForTaking(id)
      if (response.success) {
        setQuiz(response.data)
      }
    } catch (err) {
      setError(err.message || 'Failed to load quiz')
      console.error('Error:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleAnswerSelect = (questionId, answerId) => {
    setAnswers({
      ...answers,
      [questionId]: answerId
    })
  }

  const handleNext = () => {
    if (currentQuestion < quiz.questions.length - 1) {
      setCurrentQuestion(currentQuestion + 1)
    }
  }

  const handlePrevious = () => {
    if (currentQuestion > 0) {
      setCurrentQuestion(currentQuestion - 1)
    }
  }

  const handleSubmit = async () => {
    const unanswered = quiz.questions.filter(q => !answers[q.id])
    if (unanswered.length > 0) {
      if (!window.confirm(`You have ${unanswered.length} unanswered questions. Submit anyway?`)) {
        return
      }
    }

    setSubmitting(true)
    try {
      const quizData = QuizAPI.formatQuizData(id, 
        Object.entries(answers).map(([questionId, answerId]) => ({
          questionId,
          answerId
        }))
      )

      const response = await QuizAPI.submitQuiz(quizData)
      if (response.success) {
        alert(response.message)
        navigate(`/result/${response.data.id}`)
      }
    } catch (err) {
      setError(err.message || 'Failed to submit quiz')
      console.error('Error:', err)
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) return <div className={styles.loading}>Loading quiz...</div>
  if (error) return <div className={styles.error}>{error}</div>
  if (!quiz) return <div>Quiz not found</div>

  const question = quiz.questions[currentQuestion]
  const progress = ((currentQuestion + 1) / quiz.questions.length) * 100

  return (
    <div className={styles.takeQuizContainer}>
      <div className={styles.quizHeader}>
        <h2>{quiz.title}</h2>
        <div className={styles.progressBar}>
          <div className={styles.progressFill} style={{ width: `${progress}%` }}></div>
        </div>
        <div className={styles.questionCounter}>
          Question {currentQuestion + 1} of {quiz.questions.length}
        </div>
      </div>

      <div className={styles.questionContainer}>
        <h3>{question.content}</h3>
        <div className={styles.answersList}>
          {question.answers.map((answer) => (
            <label key={answer.id} className={styles.answerOption}>
              <input
                type="radio"
                name={`question-${question.id}`}
                value={answer.id}
                checked={answers[question.id] === answer.id}
                onChange={() => handleAnswerSelect(question.id, answer.id)}
              />
              <span>{answer.content}</span>
            </label>
          ))}
        </div>
      </div>

<div className={styles.quizNavigation}>
        <button 
          onClick={handlePrevious}
          disabled={currentQuestion === 0}
className={styles.btnSecondary}
        >
          Previous
        </button>
        
        {currentQuestion < quiz.questions.length - 1 ? (
          <button onClick={handleNext} className={styles.btnPrimary}>
            Next
          </button>
        ) : (
          <button 
            onClick={handleSubmit}
            disabled={submitting}
className={styles.btnSuccess}
          >
            {submitting ? 'Submitting...' : 'Submit Quiz'}
          </button>
        )}
      </div>

<div className={styles.questionOverview}>
        <h4>Question Overview</h4>
        <div className={styles.questionDots}>
          {quiz.questions.map((q, index) => (
            <button
              key={q.id}
              className={`${styles.dot} ${index === currentQuestion ? styles.active : ''} ${answers[q.id] ? styles.answered : ''}`}
              onClick={() => setCurrentQuestion(index)}
            >
              {index + 1}
            </button>
          ))}
        </div>
      </div>
    </div>
  )
}

export default TakeQuiz