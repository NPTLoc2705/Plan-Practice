import { useState } from 'react'

const apiBase = '/api/QuizManagement'

function JsonPretty({ data }) {
  if (!data) return null
  return (
    <pre style={{ background: '#111', color: '#0f0', padding: 12, whiteSpace: 'pre-wrap' }}>
      {JSON.stringify(data, null, 2)}
    </pre>
  )
}

export default function QuizManagement() {
  const [quizId, setQuizId] = useState('')
  const [userId, setUserId] = useState('')
  const [details, setDetails] = useState(null)
  const [createPayload, setCreatePayload] = useState(`{
  "title": "Sample Quiz",
  "description": "Basics",
  "questions": [
    {
      "content": "2+2?",
      "answers": [
        { "content": "3", "isCorrect": false },
        { "content": "4", "isCorrect": true }
      ]
    }
  ]
}`)
  const [created, setCreated] = useState(null)
  const [submitPayload, setSubmitPayload] = useState(`{
  "userId": 1,
  "quizId": 1,
  "answers": [
    { "questionId": 1, "answerId": 2 }
  ]
}`)
  const [submitResult, setSubmitResult] = useState(null)
  const [history, setHistory] = useState(null)
  const [stats, setStats] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  async function callApi(path, options) {
    setLoading(true)
    setError(null)
    try {
      const res = await fetch(path, {
        headers: { 'Content-Type': 'application/json' },
        ...options,
      })
      if (!res.ok) {
        const text = await res.text()
        throw new Error(text || `HTTP ${res.status}`)
      }
      const contentType = res.headers.get('content-type') || ''
      if (contentType.includes('application/json')) return await res.json()
      return await res.text()
    } catch (e) {
      setError(e.message)
      return null
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ display: 'grid', gap: 16, padding: 16 }}>
      <h2>Quiz Management</h2>

      <section style={{ border: '1px solid #444', padding: 12 }}>
        <h3>Get Quiz Details</h3>
        <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
          <input placeholder="Quiz Id" value={quizId} onChange={e => setQuizId(e.target.value)} />
          <button disabled={!quizId || loading} onClick={async () => {
            const data = await callApi(`${apiBase}/quiz/${quizId}/details`)
            setDetails(data)
          }}>Fetch</button>
        </div>
        <JsonPretty data={details} />
      </section>

      <section style={{ border: '1px solid #444', padding: 12 }}>
        <h3>Create Complete Quiz</h3>
        <textarea rows={10} value={createPayload} onChange={e => setCreatePayload(e.target.value)} style={{ width: '100%', fontFamily: 'monospace' }} />
        <div>
          <button disabled={loading} onClick={async () => {
            let body
            try { body = JSON.parse(createPayload) } catch { setError('Invalid JSON for create payload'); return }
            const data = await callApi(`${apiBase}/quiz/create-complete`, { method: 'POST', body: JSON.stringify(body) })
            setCreated(data)
          }}>Create</button>
        </div>
        <JsonPretty data={created} />
      </section>

      <section style={{ border: '1px solid #444', padding: 12 }}>
        <h3>Submit Quiz Answers</h3>
        <textarea rows={10} value={submitPayload} onChange={e => setSubmitPayload(e.target.value)} style={{ width: '100%', fontFamily: 'monospace' }} />
        <div>
          <button disabled={loading} onClick={async () => {
            let body
            try { body = JSON.parse(submitPayload) } catch { setError('Invalid JSON for submit payload'); return }
            const data = await callApi(`${apiBase}/quiz/submit`, { method: 'POST', body: JSON.stringify(body) })
            setSubmitResult(data)
          }}>Submit</button>
        </div>
        <JsonPretty data={submitResult} />
      </section>

      <section style={{ border: '1px solid #444', padding: 12 }}>
        <h3>User Quiz History</h3>
        <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
          <input placeholder="User Id" value={userId} onChange={e => setUserId(e.target.value)} />
          <button disabled={!userId || loading} onClick={async () => {
            const data = await callApi(`${apiBase}/user/${userId}/history`)
            setHistory(data)
          }}>Fetch</button>
        </div>
        <JsonPretty data={history} />
      </section>

      <section style={{ border: '1px solid #444', padding: 12 }}>
        <h3>Quiz Statistics</h3>
        <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
          <input placeholder="Quiz Id" value={quizId} onChange={e => setQuizId(e.target.value)} />
          <button disabled={!quizId || loading} onClick={async () => {
            const data = await callApi(`${apiBase}/quiz/${quizId}/statistics`)
            setStats(data)
          }}>Fetch</button>
        </div>
        <JsonPretty data={stats} />
      </section>

      {error && <div style={{ color: 'crimson' }}>Error: {error}</div>}
    </div>
  )
}



