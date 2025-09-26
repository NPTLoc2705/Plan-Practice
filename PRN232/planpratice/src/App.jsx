import './App.css'
import { BrowserRouter, Routes, Route, Link, Navigate } from 'react-router-dom'
import QuizManagement from './app/pages/QuizManagement.jsx'

function App() {
  return (
    <BrowserRouter>
      <nav style={{ display: 'flex', gap: 12, padding: 12 }}>
        <Link to="/">Home</Link>
        <Link to="/quiz">Quiz Management</Link>
      </nav>
      <Routes>
        <Route path="/" element={<div style={{ padding: 16 }}><h1>Home</h1><p>Welcome.</p></div>} />
        <Route path="/quiz/*" element={<QuizManagement />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
