import { useState } from 'react'
import { Plus, Search, BookOpen, HelpCircle, CheckCircle, BarChart3, User, Home } from 'lucide-react'
import './output.css'
const apiBase = '/api'

function JsonPretty({ data }) {
  if (!data) return null
  return (
    <pre className="bg-gray-800 text-green-400 p-4 rounded-lg overflow-auto text-sm">
      {JSON.stringify(data, null, 2)}
    </pre>
  )
}

export default function QuizManagement() {
  const [activeTab, setActiveTab] = useState('quizzes')
  const [quizId, setQuizId] = useState('')
  const [userId, setUserId] = useState('')
  const [teacherId, setTeacherId] = useState('')
  const [questionId, setQuestionId] = useState('')
  const [answerId, setAnswerId] = useState('')
  
  const [quizDetails, setQuizDetails] = useState(null)
  const [allQuizzes, setAllQuizzes] = useState(null)
  const [teacherQuizzes, setTeacherQuizzes] = useState(null)
  const [teacherDashboard, setTeacherDashboard] = useState(null)
  
  const [questionDetails, setQuestionDetails] = useState(null)
  const [allQuestions, setAllQuestions] = useState(null)
  const [quizQuestions, setQuizQuestions] = useState(null)
  
  const [answerDetails, setAnswerDetails] = useState(null)
  const [allAnswers, setAllAnswers] = useState(null)
  const [questionAnswers, setQuestionAnswers] = useState(null)
  
  const [createQuizPayload, setCreateQuizPayload] = useState(`{
  "title": "Sample Quiz",
  "description": "A sample quiz for testing"
}`)
  const [createQuestionPayload, setCreateQuestionPayload] = useState(`{
  "content": "What is 2+2?",
  "quizId": 1
}`)
  const [createAnswerPayload, setCreateAnswerPayload] = useState(`{
  "content": "4",
  "isCorrect": true,
  "questionId": 1
}`)
  
  const [created, setCreated] = useState(null)
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
    <div className="min-h-screen bg-gradient-to-br from-indigo-50 via-white to-purple-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-indigo-600 to-purple-600 rounded-lg flex items-center justify-center">
                <BookOpen className="w-6 h-6 text-white" />
              </div>
              <h1 className="text-2xl font-bold text-gray-900">QuizLab</h1>
            </div>
            <div className="flex items-center gap-4">
              <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <Search className="w-5 h-5 text-gray-600" />
              </button>
              <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <User className="w-5 h-5 text-gray-600" />
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Navigation Tabs */}
      <nav className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-6">
          <div className="flex gap-8">
            <button
              onClick={() => setActiveTab('quizzes')}
              className={`py-4 px-2 border-b-2 font-medium transition-colors ${
                activeTab === 'quizzes'
                  ? 'border-indigo-600 text-indigo-600'
                  : 'border-transparent text-gray-600 hover:text-gray-900'
              }`}
            >
              <div className="flex items-center gap-2">
                <BookOpen className="w-5 h-5" />
                Quizzes
              </div>
            </button>
            <button
              onClick={() => setActiveTab('questions')}
              className={`py-4 px-2 border-b-2 font-medium transition-colors ${
                activeTab === 'questions'
                  ? 'border-indigo-600 text-indigo-600'
                  : 'border-transparent text-gray-600 hover:text-gray-900'
              }`}
            >
              <div className="flex items-center gap-2">
                <HelpCircle className="w-5 h-5" />
                Questions
              </div>
            </button>
            <button
              onClick={() => setActiveTab('answers')}
              className={`py-4 px-2 border-b-2 font-medium transition-colors ${
                activeTab === 'answers'
                  ? 'border-indigo-600 text-indigo-600'
                  : 'border-transparent text-gray-600 hover:text-gray-900'
              }`}
            >
              <div className="flex items-center gap-2">
                <CheckCircle className="w-5 h-5" />
                Answers
              </div>
            </button>
            <button
              onClick={() => setActiveTab('dashboard')}
              className={`py-4 px-2 border-b-2 font-medium transition-colors ${
                activeTab === 'dashboard'
                  ? 'border-indigo-600 text-indigo-600'
                  : 'border-transparent text-gray-600 hover:text-gray-900'
              }`}
            >
              <div className="flex items-center gap-2">
                <BarChart3 className="w-5 h-5" />
                Dashboard
              </div>
            </button>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-6 py-8">
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-800">
            <strong>Error:</strong> {error}
          </div>
        )}

        {/* Quiz Tab */}
        {activeTab === 'quizzes' && (
          <div className="space-y-6">
            <div className="flex items-center justify-between">
              <h2 className="text-3xl font-bold text-gray-900">Quiz Management</h2>
              <button
                disabled={loading}
                onClick={async () => {
                  const data = await callApi(`${apiBase}/Quiz`)
                  setAllQuizzes(data)
                }}
                className="flex items-center gap-2 px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed shadow-sm"
              >
                <Plus className="w-5 h-5" />
                Get All Quizzes
              </button>
            </div>

            <div className="grid md:grid-cols-2 gap-6">
              {/* Get Quiz by ID */}
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Get Quiz Details</h3>
                <div className="space-y-3">
                  <input
                    type="text"
                    placeholder="Enter Quiz ID"
                    value={quizId}
                    onChange={e => setQuizId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                  />
                  <button
                    disabled={!quizId || loading}
                    onClick={async () => {
                      const data = await callApi(`${apiBase}/Quiz/${quizId}`)
                      setQuizDetails(data)
                    }}
                    className="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Fetch Quiz
                  </button>
                </div>
                {quizDetails && <div className="mt-4"><JsonPretty data={quizDetails} /></div>}
              </div>

              {/* Get Teacher Quizzes */}
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Get Teacher Quizzes</h3>
                <div className="space-y-3">
                  <input
                    type="text"
                    placeholder="Enter Teacher ID"
                    value={teacherId}
                    onChange={e => setTeacherId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                  />
                  <button
                    disabled={!teacherId || loading}
                    onClick={async () => {
                      const data = await callApi(`${apiBase}/Quiz/teacher/${teacherId}`)
                      setTeacherQuizzes(data)
                    }}
                    className="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Fetch Quizzes
                  </button>
                </div>
                {teacherQuizzes && <div className="mt-4"><JsonPretty data={teacherQuizzes} /></div>}
              </div>
            </div>

            {/* Create Quiz */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
              <h3 className="text-2xl font-bold text-gray-900 mb-2">Create a new study set</h3>
              <p className="text-gray-600 mb-8">Build your own quiz and share it with others</p>
              
              <div className="space-y-6">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Title
                  </label>
                  <input
                    type="text"
                    placeholder="Enter a title, like 'Biology - Chapter 22: Evolution'"
                    value={JSON.parse(createQuizPayload).title}
                    onChange={e => {
                      const payload = JSON.parse(createQuizPayload)
                      payload.title = e.target.value
                      setCreateQuizPayload(JSON.stringify(payload, null, 2))
                    }}
                    className="w-full px-4 py-3 border-b-2 border-gray-300 focus:border-indigo-600 outline-none text-lg transition-colors"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Description
                  </label>
                  <textarea
                    rows={3}
                    placeholder="Add a description..."
                    value={JSON.parse(createQuizPayload).description}
                    onChange={e => {
                      const payload = JSON.parse(createQuizPayload)
                      payload.description = e.target.value
                      setCreateQuizPayload(JSON.stringify(payload, null, 2))
                    }}
                    className="w-full px-4 py-3 border-b-2 border-gray-300 focus:border-indigo-600 outline-none resize-none transition-colors"
                  />
                </div>
              </div>

              <div className="mt-8 flex gap-3">
                <button
                  disabled={loading}
                  onClick={async () => {
                    let body
                    try { body = JSON.parse(createQuizPayload) } catch { setError('Invalid JSON'); return }
                    const data = await callApi(`${apiBase}/Quiz`, { method: 'POST', body: JSON.stringify(body) })
                    setCreated(data)
                  }}
                  className="px-8 py-3 bg-indigo-600 text-white font-semibold rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed shadow-sm"
                >
                  Create
                </button>
                <button
                  onClick={() => setCreateQuizPayload(`{
  "title": "Sample Quiz",
  "description": "A sample quiz for testing"
}`)}
                  className="px-8 py-3 bg-gray-100 text-gray-700 font-semibold rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Reset
                </button>
              </div>
              
              {created && (
                <div className="mt-6 p-4 bg-green-50 border border-green-200 rounded-lg">
                  <p className="text-green-800 font-semibold mb-2">✓ Quiz created successfully!</p>
                  <JsonPretty data={created} />
                </div>
              )}
            </div>

            {allQuizzes && (
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">All Quizzes</h3>
                <JsonPretty data={allQuizzes} />
              </div>
            )}
          </div>
        )}

        {/* Questions Tab */}
        {activeTab === 'questions' && (
          <div className="space-y-6">
            <div className="flex items-center justify-between">
              <h2 className="text-3xl font-bold text-gray-900">Question Management</h2>
              <button
                disabled={loading}
                onClick={async () => {
                  const data = await callApi(`${apiBase}/Question`)
                  setAllQuestions(data)
                }}
                className="flex items-center gap-2 px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 shadow-sm"
              >
                <Plus className="w-5 h-5" />
                Get All Questions
              </button>
            </div>

            <div className="grid md:grid-cols-2 gap-6">
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Get Question Details</h3>
                <div className="space-y-3">
                  <input
                    type="text"
                    placeholder="Enter Question ID"
                    value={questionId}
                    onChange={e => setQuestionId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                  />
                  <button
                    disabled={!questionId || loading}
                    onClick={async () => {
                      const data = await callApi(`${apiBase}/Question/${questionId}`)
                      setQuestionDetails(data)
                    }}
                    className="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50"
                  >
                    Fetch Question
                  </button>
                </div>
                {questionDetails && <div className="mt-4"><JsonPretty data={questionDetails} /></div>}
              </div>

              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Get Quiz Questions</h3>
                <div className="space-y-3">
                  <input
                    type="text"
                    placeholder="Enter Quiz ID"
                    value={quizId}
                    onChange={e => setQuizId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                  />
                  <button
                    disabled={!quizId || loading}
                    onClick={async () => {
                      const data = await callApi(`${apiBase}/Question/quiz/${quizId}`)
                      setQuizQuestions(data)
                    }}
                    className="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50"
                  >
                    Fetch Questions
                  </button>
                </div>
                {quizQuestions && <div className="mt-4"><JsonPretty data={quizQuestions} /></div>}
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
              <h3 className="text-2xl font-bold text-gray-900 mb-2">Create a new question</h3>
              <p className="text-gray-600 mb-8">Add a question to your quiz</p>
              
              <div className="space-y-6">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Question Content
                  </label>
                  <textarea
                    rows={3}
                    placeholder="Enter your question here..."
                    value={JSON.parse(createQuestionPayload).content}
                    onChange={e => {
                      const payload = JSON.parse(createQuestionPayload)
                      payload.content = e.target.value
                      setCreateQuestionPayload(JSON.stringify(payload, null, 2))
                    }}
                    className="w-full px-4 py-3 border-b-2 border-gray-300 focus:border-indigo-600 outline-none text-lg resize-none transition-colors"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Quiz ID
                  </label>
                  <input
                    type="number"
                    placeholder="Enter quiz ID"
                    value={JSON.parse(createQuestionPayload).quizId}
                    onChange={e => {
                      const payload = JSON.parse(createQuestionPayload)
                      payload.quizId = parseInt(e.target.value) || 1
                      setCreateQuestionPayload(JSON.stringify(payload, null, 2))
                    }}
                    className="w-full px-4 py-3 border-b-2 border-gray-300 focus:border-indigo-600 outline-none text-lg transition-colors"
                  />
                </div>
              </div>

              <div className="mt-8 flex gap-3">
                <button
                  disabled={loading}
                  onClick={async () => {
                    let body
                    try { body = JSON.parse(createQuestionPayload) } catch { setError('Invalid JSON'); return }
                    const data = await callApi(`${apiBase}/Question`, { method: 'POST', body: JSON.stringify(body) })
                    setCreated(data)
                  }}
                  className="px-8 py-3 bg-indigo-600 text-white font-semibold rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed shadow-sm"
                >
                  Create
                </button>
                <button
                  onClick={() => setCreateQuestionPayload(`{
  "content": "What is 2+2?",
  "quizId": 1
}`)}
                  className="px-8 py-3 bg-gray-100 text-gray-700 font-semibold rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Reset
                </button>
              </div>
              
              {created && (
                <div className="mt-6 p-4 bg-green-50 border border-green-200 rounded-lg">
                  <p className="text-green-800 font-semibold mb-2">✓ Question created successfully!</p>
                  <JsonPretty data={created} />
                </div>
              )}
            </div>

            {allQuestions && (
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">All Questions</h3>
                <JsonPretty data={allQuestions} />
              </div>
            )}
          </div>
        )}

        {/* Answers Tab */}
        {activeTab === 'answers' && (
          <div className="space-y-6">
            <div className="flex items-center justify-between">
              <h2 className="text-3xl font-bold text-gray-900">Answer Management</h2>
              <button
                disabled={loading}
                onClick={async () => {
                  const data = await callApi(`${apiBase}/Answer`)
                  setAllAnswers(data)
                }}
                className="flex items-center gap-2 px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 shadow-sm"
              >
                <Plus className="w-5 h-5" />
                Get All Answers
              </button>
            </div>

            <div className="grid md:grid-cols-2 gap-6">
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Get Answer Details</h3>
                <div className="space-y-3">
                  <input
                    type="text"
                    placeholder="Enter Answer ID"
                    value={answerId}
                    onChange={e => setAnswerId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                  />
                  <button
                    disabled={!answerId || loading}
                    onClick={async () => {
                      const data = await callApi(`${apiBase}/Answer/${answerId}`)
                      setAnswerDetails(data)
                    }}
                    className="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50"
                  >
                    Fetch Answer
                  </button>
                </div>
                {answerDetails && <div className="mt-4"><JsonPretty data={answerDetails} /></div>}
              </div>

              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Get Question Answers</h3>
                <div className="space-y-3">
                  <input
                    type="text"
                    placeholder="Enter Question ID"
                    value={questionId}
                    onChange={e => setQuestionId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                  />
                  <button
                    disabled={!questionId || loading}
                    onClick={async () => {
                      const data = await callApi(`${apiBase}/Answer/question/${questionId}`)
                      setQuestionAnswers(data)
                    }}
                    className="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50"
                  >
                    Fetch Answers
                  </button>
                </div>
                {questionAnswers && <div className="mt-4"><JsonPretty data={questionAnswers} /></div>}
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
              <h3 className="text-2xl font-bold text-gray-900 mb-2">Create a new answer</h3>
              <p className="text-gray-600 mb-8">Add an answer option to a question</p>
              
              <div className="space-y-6">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Answer Content
                  </label>
                  <input
                    type="text"
                    placeholder="Enter the answer..."
                    value={JSON.parse(createAnswerPayload).content}
                    onChange={e => {
                      const payload = JSON.parse(createAnswerPayload)
                      payload.content = e.target.value
                      setCreateAnswerPayload(JSON.stringify(payload, null, 2))
                    }}
                    className="w-full px-4 py-3 border-b-2 border-gray-300 focus:border-indigo-600 outline-none text-lg transition-colors"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Question ID
                  </label>
                  <input
                    type="number"
                    placeholder="Enter question ID"
                    value={JSON.parse(createAnswerPayload).questionId}
                    onChange={e => {
                      const payload = JSON.parse(createAnswerPayload)
                      payload.questionId = parseInt(e.target.value) || 1
                      setCreateAnswerPayload(JSON.stringify(payload, null, 2))
                    }}
                    className="w-full px-4 py-3 border-b-2 border-gray-300 focus:border-indigo-600 outline-none text-lg transition-colors"
                  />
                </div>

                <div>
                  <label className="flex items-center gap-3 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={JSON.parse(createAnswerPayload).isCorrect}
                      onChange={e => {
                        const payload = JSON.parse(createAnswerPayload)
                        payload.isCorrect = e.target.checked
                        setCreateAnswerPayload(JSON.stringify(payload, null, 2))
                      }}
                      className="w-5 h-5 text-indigo-600 rounded focus:ring-2 focus:ring-indigo-500"
                    />
                    <span className="text-sm font-semibold text-gray-700">This is the correct answer</span>
                  </label>
                </div>
              </div>

              <div className="mt-8 flex gap-3">
                <button
                  disabled={loading}
                  onClick={async () => {
                    let body
                    try { body = JSON.parse(createAnswerPayload) } catch { setError('Invalid JSON'); return }
                    const data = await callApi(`${apiBase}/Answer`, { method: 'POST', body: JSON.stringify(body) })
                    setCreated(data)
                  }}
                  className="px-8 py-3 bg-indigo-600 text-white font-semibold rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed shadow-sm"
                >
                  Create
                </button>
                <button
                  onClick={() => setCreateAnswerPayload(`{
  "content": "4",
  "isCorrect": true,
  "questionId": 1
}`)}
                  className="px-8 py-3 bg-gray-100 text-gray-700 font-semibold rounded-lg hover:bg-gray-200 transition-colors"
                >
                  Reset
                </button>
              </div>
              
              {created && (
                <div className="mt-6 p-4 bg-green-50 border border-green-200 rounded-lg">
                  <p className="text-green-800 font-semibold mb-2">✓ Answer created successfully!</p>
                  <JsonPretty data={created} />
                </div>
              )}
            </div>

            {allAnswers && (
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">All Answers</h3>
                <JsonPretty data={allAnswers} />
              </div>
            )}
          </div>
        )}

        {/* Dashboard Tab */}
        {activeTab === 'dashboard' && (
          <div className="space-y-6">
            <h2 className="text-3xl font-bold text-gray-900">Teacher Dashboard</h2>
            
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Get Dashboard Data</h3>
              <div className="space-y-3">
                <input
                  type="text"
                  placeholder="Enter Teacher ID"
                  value={teacherId}
                  onChange={e => setTeacherId(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                />
                <button
                  disabled={!teacherId || loading}
                  onClick={async () => {
                    const data = await callApi(`${apiBase}/Quiz/teacher-dashboard/${teacherId}`)
                    setTeacherDashboard(data)
                  }}
                  className="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50"
                >
                  Load Dashboard
                </button>
              </div>
              {teacherDashboard && <div className="mt-4"><JsonPretty data={teacherDashboard} /></div>}
            </div>
          </div>
        )}
      </main>
    </div>
  )
}