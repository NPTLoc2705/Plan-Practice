import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import QuizAPI from '../components/APIService/StudentQuizAPI'  
import styles from './QuizHistory.module.css'
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
    navigate(`/student/result/${resultId}`)
  }

if (loading) return <div className={styles.loading}>Loading history...</div>
if (error) return <div className={styles.error}>{error}</div>

  return (
    <div className={styles.historyContainer}>
      <h2>Quiz History</h2>
      
      {history.length === 0 ? (
        <div className={styles.noHistory}>
          <p>You haven't taken any quizzes yet.</p>
            
        </div>
      ) : (
        <>
<div className={styles.historyTable}>
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
className={styles.btnSmall}
                      >
                        View Details
                      </button>
                      <button 
                        onClick={() => fetchStatistics(item.quizId)}
className={styles.btnSmallSecondary}
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
<div className={styles.statisticsPanel}>
              <h3>Quiz Statistics: {statistics.quizTitle}</h3>
              <button 
                onClick={() => setStatistics(null)}
className={styles.closeBtn}
              >
                ×
              </button>
              <div className={styles.statsGrid}>
                <div className={styles.statItem}>
                  <label>Your Best Score:</label>
                  <span>{statistics.yourBestScore}</span>
                </div>
                <div className={styles.statItem}>
                  <label>Your Attempts:</label>
                  <span>{statistics.yourAttempts}</span>
                </div>
                <div className={styles.statItem}>
                  <label>Total Attempts (All Users):</label>
                  <span>{statistics.totalAttempts}</span>
                </div>
                <div className={styles.statItem}>
                  <label>Average Score:</label>
                  <span>{statistics.averageScore?.toFixed(1)}</span>
                </div>
                <div className={styles.statItem}>
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