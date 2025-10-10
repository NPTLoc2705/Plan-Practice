import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import QuizAPI from '../components/APIService/StudentQuizAPI'  
import styles from './QuizResult.module.css'
function QuizResult() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [result, setResult] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchResult()
  }, [id])

  const fetchResult = async () => {
    try {
      setLoading(true)
      const response = await QuizAPI.getQuizResult(id)
      if (response.success) {
        setResult(response.data)
      }
    } catch (err) {
      setError(err.message || 'Failed to load result')
      console.error('Error:', err)
    } finally {
      setLoading(false)
    }
  }

if (loading) return <div className={styles.loading}>Loading result...</div>
if (error) return <div className={styles.error}>{error}</div>
  if (!result) return <div>Result not found</div>

  const scoreColorClass = QuizAPI.getScoreColorClass(result.percentage)

  return (
    <div className={styles.quizResultContainer}>
      <div className={styles.resultHeader}>
        <h2>Quiz Result</h2>
        <h3>{result.quizTitle}</h3>
      </div>

      <div className={styles.scoreCard}>
        <div className={`${styles.scoreCircle} ${scoreColorClass}`}>
          <div className={styles.scoreValue}>{result.percentage}%</div>
          <div className={styles.scoreLabel}>
            {result.score}/{result.totalQuestions}
          </div>
        </div>
        <div className={styles.scoreSummary}>
          <div className={styles.summaryItem}>
            <span>Correct Answers:</span>
            <span>{result.correctAnswers}</span>
          </div>
          <div className={styles.summaryItem}>
            <span>Total Questions:</span>
            <span>{result.totalQuestions}</span>
          </div>
          <div className={styles.summaryItem}>
            <span>Completed at:</span>
            <span>{QuizAPI.formatDate(result.completedAt)}</span>
          </div>
        </div>
      </div>

      {result.details && (
<div className={styles.answerReview}>
          <h3>Answer Review</h3>
          {result.details.map((detail, index) => (
            <div key={detail.questionId} className={`${styles.reviewItem} ${detail.isCorrect ? styles.correct : styles.incorrect}`}>
              <div className={styles.questionNumber}>Question {index + 1}</div>
              <div className={styles.questionContent}>{detail.questionContent}</div>
              <div className={styles.answerInfo}>
                <div className={styles.yourAnswer}>
                  <span>Your Answer:</span>
                  <span className={detail.isCorrect ? styles.textSuccess : styles.textDanger}>
                    {detail.userAnswerContent || 'Not answered'}
                  </span>
                </div>
                {!detail.isCorrect && (
                  <div className={styles.correctAnswer}>
                    <span>Correct Answer:</span>
                    <span className={styles.textSuccess}>{detail.correctAnswerContent}</span>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      <div className={styles.resultActions}>
        <button onClick={() => navigate('/quizzes')} className={styles.btnPrimary}>
          Back to Quizzes
        </button>
        <button onClick={() => navigate('/history')} className={styles.btnSecondary}>
          View History
        </button>
      </div>
    </div>
  )
}

export default QuizResult