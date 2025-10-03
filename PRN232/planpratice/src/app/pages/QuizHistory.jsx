import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import QuizAPI from '../components/APIService/StudentQuizAPI'  

function QuizHistory() {
  const [history, setHistory] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [selectedQuiz, setSelectedQuiz] = useState(null)
  const [statistics, setStatistics] = useState(null)
  const navigate = useNavigate()

  useEffect(() => {
    fetchHistory()
  }, [])

  const fetchHistory = async () => {
    try {
      setLoading(true)
      const response = await QuizAPI.getQuizHistory()
      if (response.success) {
        setHistory(response.data || [])
      }
    } catch (err) {
      setError(err.message || 'Failed to load history')
      console.error('Error:', err)
    } finally {
      setLoading(false)
    }
  }

  const fetchStatistics = async (quizId) => {
    try {
      const response = await QuizAPI.getQuizStatistics(quizId)
      if (response.success) {
        setStatistics(response.data)
        setSelectedQuiz(quizId)
      }
    } catch (err) {
      console.error('Failed to load statistics', err)
    }
  }

  const viewResult = (resultId) => {
    navigate(`/result/${resultId}`)
  }

  if (loading) return <div className="loading">Loading history...</div>
  if (error) return <div className="error">{error}</div>

  return (
    <div className="history-container">
      <h2>Quiz History</h2>
      
      {history.length === 0 ? (
        <div className="no-history">
          <p>You haven't taken any quizzes yet.</p>
          <button onClick={() => navigate('/quizzes')} className="btn-primary">
            Browse Quizzes
          </button>
        </div>
      ) : (
        <>
          <div className="history-table">
            <table>
              <thead>
                <tr>
                  <th>Quiz Title</th>
                  <th>Score</th>
                  <th>Percentage</th>
                  <th>Date</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {history.map((item) => (
                  <tr key={item.resultId}>
                    <td>{item.quizTitle}</td>
                    <td>{item.score}/{item.totalQuestions}</td>
                    <td className={QuizAPI.getScoreColorClass(item.percentage)}>
                      {item.percentage}%
                    </td>
                    <td>{QuizAPI.formatDate(item.completedAt)}</td>
                    <td>
                      <button 
                        onClick={() => viewResult(item.resultId)}
                        className="btn-small"
                      >
                        View Details
                      </button>
                      <button 
                        onClick={() => fetchStatistics(item.quizId)}
                        className="btn-small-secondary"
                      >
                        Statistics
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {statistics && selectedQuiz && (
            <div className="statistics-panel">
              <h3>Quiz Statistics: {statistics.quizTitle}</h3>
              <button 
                onClick={() => setStatistics(null)}
                className="close-btn"
              >
                ×
              </button>
              <div className="stats-grid">
                <div className="stat-item">
                  <label>Your Best Score:</label>
                  <span>{statistics.yourBestScore}</span>
                </div>
                <div className="stat-item">
                  <label>Your Attempts:</label>
                  <span>{statistics.yourAttempts}</span>
                </div>
                <div className="stat-item">
                  <label>Total Attempts (All Users):</label>
                  <span>{statistics.totalAttempts}</span>
                </div>
                <div className="stat-item">
                  <label>Average Score:</label>
                  <span>{statistics.averageScore?.toFixed(1)}</span>
                </div>
                <div className="stat-item">
                  <label>Highest Score:</label>
                  <span>{statistics.highestScore}</span>
                </div>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  )
}

export default QuizHistory