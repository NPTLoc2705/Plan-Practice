import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import QuizAPI from '../components/APIService/StudentQuizAPI'  

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

  if (loading) return <div className="loading">Loading result...</div>
  if (error) return <div className="error">{error}</div>
  if (!result) return <div>Result not found</div>

  const scoreColorClass = QuizAPI.getScoreColorClass(result.percentage)

  return (
    <div className="result-container">
      <div className="result-header">
        <h2>Quiz Result</h2>
        <h3>{result.quizTitle}</h3>
      </div>

      <div className="score-card">
        <div className={`score-circle ${scoreColorClass}`}>
          <div className="score-value">{result.percentage}%</div>
          <div className="score-label">
            {result.score}/{result.totalQuestions}
          </div>
        </div>
        <div className="score-summary">
          <div className="summary-item">
            <span>Correct Answers:</span>
            <span>{result.correctAnswers}</span>
          </div>
          <div className="summary-item">
            <span>Total Questions:</span>
            <span>{result.totalQuestions}</span>
          </div>
          <div className="summary-item">
            <span>Completed at:</span>
            <span>{QuizAPI.formatDate(result.completedAt)}</span>
          </div>
        </div>
      </div>

      {result.details && (
        <div className="answer-review">
          <h3>Answer Review</h3>
          {result.details.map((detail, index) => (
            <div key={detail.questionId} className={`review-item ${detail.isCorrect ? 'correct' : 'incorrect'}`}>
              <div className="question-number">Question {index + 1}</div>
              <div className="question-content">{detail.questionContent}</div>
              <div className="answer-info">
                <div className="your-answer">
                  <span>Your Answer:</span>
                  <span className={detail.isCorrect ? 'text-success' : 'text-danger'}>
                    {detail.userAnswerContent || 'Not answered'}
                  </span>
                </div>
                {!detail.isCorrect && (
                  <div className="correct-answer">
                    <span>Correct Answer:</span>
                    <span className="text-success">{detail.correctAnswerContent}</span>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      <div className="result-actions">
        <button onClick={() => navigate('/quizzes')} className="btn-primary">
          Back to Quizzes
        </button>
        <button onClick={() => navigate('/history')} className="btn-secondary">
          View History
        </button>
      </div>
    </div>
  )
}

export default QuizResult