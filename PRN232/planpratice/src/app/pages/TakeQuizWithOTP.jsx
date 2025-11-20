import React, { useState, useEffect, useCallback } from 'react';
import QuizAPI from '../components/APIService/StudentQuizAPI';
import QuizOTPAPI from '../components/APIService/QuizOTPAPI';
import styles from './TakeQuizWithOTP.module.css';

const STORAGE_KEY = 'quizOTP_progress';

const TakeQuizWithOTP = () => {
  // States for OTP entry
  const [otpCode, setOtpCode] = useState('');
  const [isValidatingOTP, setIsValidatingOTP] = useState(false);
  const [otpError, setOtpError] = useState('');

  // States for quiz
  const [quiz, setQuiz] = useState(null);
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
  const [answers, setAnswers] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showSidebar, setShowSidebar] = useState(true);
  const [timeSpent, setTimeSpent] = useState(0);
  const [lastSaved, setLastSaved] = useState(null);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
  const [showResumeDialog, setShowResumeDialog] = useState(false);
  const [savedProgress, setSavedProgress] = useState(null);

  // Load saved progress on mount
 useEffect(() => {
  const saved = localStorage.getItem(STORAGE_KEY);
  if (saved) {
    try {
      const data = JSON.parse(saved);

      if (data.isSubmitted) {
        localStorage.removeItem(STORAGE_KEY);
        return;
      }

      setSavedProgress(data);
      setShowResumeDialog(true);
    } catch (error) {
      console.error('Error loading saved progress:', error);
      localStorage.removeItem(STORAGE_KEY);
    }
  }
}, []);

  // Auto-save progress
  const saveProgress = useCallback(() => {
    if (quiz) {
      const progressData = {
        otpCode,
        quiz,
        currentQuestionIndex,
        answers,
        timeSpent,
        timestamp: new Date().toISOString(),
        isSubmitted: false
      };
localStorage.setItem(STORAGE_KEY, JSON.stringify(progressData));
      setLastSaved(new Date());
      setHasUnsavedChanges(false);
    }
  }, [quiz, currentQuestionIndex, answers, timeSpent, otpCode]);

  // Auto-save when answers change
  useEffect(() => {
    if (quiz && Object.keys(answers).length > 0) {
      const timer = setTimeout(() => {
        saveProgress();
      }, 1000); // Auto-save after 1 second of no changes
      return () => clearTimeout(timer);
    }
  }, [answers, saveProgress, quiz]);

  // Timer
  useEffect(() => {
    if (quiz) {
      const timer = setInterval(() => {
        setTimeSpent(prev => prev + 1);
      }, 1000);
      return () => clearInterval(timer);
    }
  }, [quiz]);

  // Prevent accidental page close
  useEffect(() => {
    if (quiz) {
      const handleBeforeUnload = (e) => {
        saveProgress();
        e.preventDefault();
        e.returnValue = 'You have an active quiz. Are you sure you want to leave?';
      };
      window.addEventListener('beforeunload', handleBeforeUnload);
      return () => window.removeEventListener('beforeunload', handleBeforeUnload);
    }
  }, [quiz, saveProgress]);

  // Resume saved progress
  const handleResumeProgress = () => {
    if (savedProgress) {
      setOtpCode(savedProgress.otpCode);
      setQuiz(savedProgress.quiz);
      setCurrentQuestionIndex(savedProgress.currentQuestionIndex);
      setAnswers(savedProgress.answers);
      setTimeSpent(savedProgress.timeSpent);
      setShowResumeDialog(false);
    }
  };

  // Start fresh
  const handleStartFresh = () => {
    localStorage.removeItem(STORAGE_KEY);
    setSavedProgress(null);
    setShowResumeDialog(false);
  };

  // Clear current session
  const handleClearSession = () => {
    if (window.confirm('Are you sure you want to clear your progress and start over?')) {
      localStorage.removeItem(STORAGE_KEY);
      setQuiz(null);
      setOtpCode('');
      setAnswers({});
      setCurrentQuestionIndex(0);
      setTimeSpent(0);
    }
  };

  const handleGetQuiz = async (e) => {
    e.preventDefault();

    if (!otpCode.trim()) {
      setOtpError('Please enter an OTP code');
      return;
    }

    setIsValidatingOTP(true);
    setOtpError('');

    try {
      const response = await QuizOTPAPI.getQuizByOTP(otpCode.toUpperCase());

      if (response.success) {
        setQuiz(response.data);
        setAnswers({});
        setCurrentQuestionIndex(0);
        setTimeSpent(0);
        saveProgress();
      }
    } catch (error) {
      setOtpError(error.message);
    } finally {
      setIsValidatingOTP(false);
    }
  };

  const handleAnswerSelect = (questionId, answerId) => {
    setAnswers(prev => ({
      ...prev,
      [questionId]: answerId
    }));
    setHasUnsavedChanges(true);
  };

  const goToNextQuestion = () => {
    if (currentQuestionIndex < quiz.questions.length - 1) {
      setCurrentQuestionIndex(prev => prev + 1);
    }
  };

  const goToPreviousQuestion = () => {
    if (currentQuestionIndex > 0) {
      setCurrentQuestionIndex(prev => prev - 1);
    }
  };

  const goToQuestion = (index) => {
    setCurrentQuestionIndex(index);
  };

  const handleSubmitQuiz = async () => {
  const unansweredQuestions = quiz.questions.filter(q => !answers[q.id]);

  if (unansweredQuestions.length > 0) {
    if (!window.confirm(
      `You have ${unansweredQuestions.length} unanswered question(s). Do you want to submit anyway?`
    )) {
      return;
    }
  }

  setIsSubmitting(true);

  try {
    const formattedAnswers = Object.entries(answers).map(([questionId, answerId]) => ({
      questionId: parseInt(questionId),
      answerId: parseInt(answerId)
    }));

    const quizData = QuizAPI.formatQuizData(quiz.id, formattedAnswers);
    const response = await QuizAPI.submitQuiz(quizData);

    if (response.success) {
      // 1. XÓA SẠCH localStorage TRƯỚC KHI HIỆN ALERT
      localStorage.removeItem(STORAGE_KEY);

      // 2. Reset toàn bộ state về ban đầu
      setQuiz(null);
      setOtpCode('');
      setAnswers({});
      setCurrentQuestionIndex(0);
      setTimeSpent(0);
      setLastSaved(null);
      setHasUnsavedChanges(false);

      // 3. Dùng setTimeout để đảm bảo UI đã cập nhật xong mới alert
      setTimeout(() => {
        alert('Quiz submitted successfully! 🎉\nYou can now close this tab.');
      }, 100);
    }
  } catch (error) {
    console.error('Submit error:', error);
    alert('Submission failed: ' + error.message);
  } finally {
    setIsSubmitting(false);
  }
};

  const formatTime = (seconds) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  const getAnsweredCount = () => {
    return Object.keys(answers).length;
  };

  const formatLastSaved = () => {
    if (!lastSaved) return 'Not saved';
    const seconds = Math.floor((new Date() - lastSaved) / 1000);
    if (seconds < 5) return 'Just now';
    if (seconds < 60) return `${seconds}s ago`;
    const minutes = Math.floor(seconds / 60);
    return `${minutes}m ago`;
  };

  // Resume Dialog
  if (showResumeDialog && savedProgress) {
    const savedDate = new Date(savedProgress.timestamp);
    const answeredCount = Object.keys(savedProgress.answers).length;

    return (
      <div className={styles.resumeOverlay}>
        <div className={styles.resumeDialog}>
          <div className={styles.resumeIcon}>📋</div>
          <h2>Resume Previous Quiz?</h2>
          <p>We found a quiz in progress. Would you like to continue where you left off?</p>

          <div className={styles.resumeDetails}>
            <div className={styles.resumeDetailItem}>
              <span className={styles.resumeLabel}>Quiz:</span>
              <span className={styles.resumeValue}>{savedProgress.quiz.title}</span>
            </div>
            <div className={styles.resumeDetailItem}>
              <span className={styles.resumeLabel}>Progress:</span>
              <span className={styles.resumeValue}>
                {answeredCount}/{savedProgress.quiz.questions.length} answered
              </span>
            </div>
            <div className={styles.resumeDetailItem}>
              <span className={styles.resumeLabel}>Time Spent:</span>
              <span className={styles.resumeValue}>{formatTime(savedProgress.timeSpent)}</span>
            </div>
            <div className={styles.resumeDetailItem}>
              <span className={styles.resumeLabel}>Last Saved:</span>
              <span className={styles.resumeValue}>
                {savedDate.toLocaleString()}
              </span>
            </div>
          </div>

          <div className={styles.resumeActions}>
            <button
              onClick={handleResumeProgress}
              className={styles.btnPrimary}
            >
              Continue Quiz
            </button>
            <button
              onClick={handleStartFresh}
              className={styles.btnSecondary}
            >
              Start Fresh
            </button>
          </div>
        </div>
      </div>
    );
  }

  // OTP Entry Screen
  if (!quiz) {
    return (
      <div className={styles.otpContainer}>
        <div className={styles.otpBackground}>
          <div className={styles.floatingShape} style={{ top: '10%', left: '10%' }}></div>
          <div className={styles.floatingShape} style={{ top: '60%', right: '15%' }}></div>
          <div className={styles.floatingShape} style={{ bottom: '10%', left: '30%' }}></div>
        </div>

        <div className={styles.otpCard}>
          <div className={styles.otpIconWrapper}>
            <div className={styles.otpIcon}>🔐</div>
          </div>

          <div className={styles.otpHeader}>
            <h2>Enter Quiz Access Code</h2>
            <p>Enter the OTP code provided by your teacher</p>
          </div>

          <form onSubmit={handleGetQuiz} className={styles.otpForm}>
            <div className={styles.inputGroup}>
              <input
                type="text"
                value={otpCode}
                onChange={(e) => setOtpCode(e.target.value.toUpperCase())}
                placeholder="ENTER CODE"
                className={styles.otpInput}
                maxLength="10"
                disabled={isValidatingOTP}
                autoFocus
              />
              <div className={styles.inputUnderline}></div>
            </div>

            {otpError && (
              <div className={styles.errorMessage}>
                <span className={styles.errorIcon}>⚠️</span>
                <span>{otpError}</span>
              </div>
            )}

            <button
              type="submit"
              className={styles.btnPrimary}
              disabled={isValidatingOTP || !otpCode.trim()}
            >
              {isValidatingOTP ? (
                <>
                  <span className={styles.spinner}></span>
                  Validating...
                </>
              ) : (
                <>
                  Access Quiz
                  <span>→</span>
                </>
              )}
            </button>
          </form>

          <div className={styles.otpHelp}>
            <div className={styles.helpIcon}>💡</div>
            <p>Don't have an access code? Contact your teacher to get one.</p>
          </div>
        </div>
      </div>
    );
  }

  // Quiz Taking Screen
  const currentQuestion = quiz.questions[currentQuestionIndex];
  const isLastQuestion = currentQuestionIndex === quiz.questions.length - 1;
  const answeredCount = getAnsweredCount();
  const progress = (answeredCount / quiz.questions.length) * 100;

  return (
    <div className={styles.pageWrapper}>
      {/* Top Header Bar */}
      <div className={styles.topBar}>
        <div className={styles.topBarContent}>
          <div className={styles.quizTitleSection}>
            <h1>{quiz.title}</h1>
            {quiz.description && <span className={styles.quizSubtitle}>{quiz.description}</span>}
          </div>

          <div className={styles.quizStats}>
            <div className={styles.statItem}>
              <span className={styles.statIcon}>⏱️</span>
              <div className={styles.statInfo}>
                <span className={styles.statLabel}>Time</span>
                <span className={styles.statValue}>{formatTime(timeSpent)}</span>
              </div>
            </div>
            <div className={styles.statItem}>
              <span className={styles.statIcon}>✓</span>
              <div className={styles.statInfo}>
                <span className={styles.statLabel}>Progress</span>
                <span className={styles.statValue}>{answeredCount}/{quiz.questions.length}</span>
              </div>
            </div>
            <div className={styles.statItem}>
              <span className={styles.statIcon}>
                {hasUnsavedChanges ? '💾' : '✅'}
              </span>
              <div className={styles.statInfo}>
                <span className={styles.statLabel}>Status</span>
                <span className={styles.statValue}>{formatLastSaved()}</span>
              </div>
            </div>
          </div>
        </div>
        <div className={styles.progressBarTop}>
          <div className={styles.progressFillTop} style={{ width: `${progress}%` }}></div>
        </div>
      </div>

      <div className={styles.mainContent}>
        {/* Sidebar */}
        <aside className={`${styles.sidebar} ${showSidebar ? styles.sidebarOpen : ''}`}>
          <div className={styles.sidebarHeader}>
            <h3>Questions</h3>
            <button
              className={styles.sidebarToggle}
              onClick={() => setShowSidebar(!showSidebar)}
            >
              {showSidebar ? '←' : '→'}
            </button>
          </div>

          <div className={styles.questionGrid}>
            {quiz.questions.map((q, index) => (
              <button
                key={q.id}
                className={`${styles.questionGridItem} 
                  ${index === currentQuestionIndex ? styles.gridItemActive : ''} 
                  ${answers[q.id] ? styles.gridItemAnswered : ''}`}
                onClick={() => goToQuestion(index)}
                title={`Question ${index + 1}${answers[q.id] ? ' - Answered' : ''}`}
              >
                <span className={styles.questionNumber}>{index + 1}</span>
                {answers[q.id] && <span className={styles.checkMark}>✓</span>}
              </button>
            ))}
          </div>

          <div className={styles.sidebarFooter}>
            <div className={styles.legend}>
              <div className={styles.legendItem}>
                <span className={`${styles.legendDot} ${styles.legendAnswered}`}></span>
                <span>Answered</span>
              </div>
              <div className={styles.legendItem}>
                <span className={`${styles.legendDot} ${styles.legendCurrent}`}></span>
                <span>Current</span>
              </div>
              <div className={styles.legendItem}>
                <span className={`${styles.legendDot} ${styles.legendUnanswered}`}></span>
                <span>Unanswered</span>
              </div>
            </div>

            <button
              onClick={handleClearSession}
              className={styles.btnDanger}
            >
              Clear Progress
            </button>
          </div>
        </aside>

        {/* Main Question Area */}
        <main className={styles.questionArea}>
          <div className={styles.questionCard}>
            <div className={styles.questionHeader}>
              <span className={styles.questionBadge}>
                Question {currentQuestionIndex + 1} of {quiz.questions.length}
              </span>
              {answers[currentQuestion.id] && (
                <span className={styles.answeredBadge}>
                  ✓ Answered
                </span>
              )}
            </div>

            <div className={styles.questionContent}>
              <h2>{currentQuestion.content}</h2>
            </div>

            <div className={styles.answersGrid}>
              {currentQuestion.answers.map((answer, index) => {
                const isSelected = answers[currentQuestion.id] === answer.id;
                const letters = ['A', 'B', 'C', 'D', 'E', 'F'];

                return (
                  <label
                    key={answer.id}
                    className={`${styles.answerCard} ${isSelected ? styles.answerSelected : ''}`}
                  >
                    <input
                      type="radio"
                      name={`question-${currentQuestion.id}`}
                      value={answer.id}
                      checked={isSelected}
                      onChange={() => handleAnswerSelect(currentQuestion.id, answer.id)}
                      className={styles.answerInput}
                    />
                    <div className={styles.answerLetter}>{letters[index]}</div>
                    <div className={styles.answerText}>{answer.content}</div>
                    <div className={styles.answerCheck}>
                      {isSelected && <span>✓</span>}
                    </div>
                  </label>
                );
              })}
            </div>
          </div>

          {/* Navigation */}
          <div className={styles.navigationBar}>
            <button
              onClick={goToPreviousQuestion}
              disabled={currentQuestionIndex === 0}
              className={styles.btnSecondary}
            >
              <span>←</span> Previous
            </button>

            <div className={styles.pageIndicator}>
              {currentQuestionIndex + 1} / {quiz.questions.length}
            </div>

            {!isLastQuestion ? (
              <button
                onClick={goToNextQuestion}
                className={styles.btnPrimary}
              >
                Next <span>→</span>
              </button>
            ) : (
              <button
                onClick={handleSubmitQuiz}
                disabled={isSubmitting}
                className={styles.btnSuccess}
              >
                {isSubmitting ? (
                  <>
                    <span className={styles.spinner}></span>
                    Submitting...
                  </>
                ) : (
                  <>
                    Submit Quiz <span>✓</span>
                  </>
                )}
              </button>
            )}
          </div>

          {/* Quick Stats Card */}
          <div className={styles.statsCard}>
            <div className={styles.statsCardItem}>
              <div className={styles.statsCardIcon}>📝</div>
              <div className={styles.statsCardInfo}>
                <span className={styles.statsCardLabel}>Total Questions</span>
                <span className={styles.statsCardValue}>{quiz.questions.length}</span>
              </div>
            </div>
            <div className={styles.statsCardItem}>
              <div className={styles.statsCardIcon}>✅</div>
              <div className={styles.statsCardInfo}>
                <span className={styles.statsCardLabel}>Answered</span>
                <span className={styles.statsCardValue}>{answeredCount}</span>
              </div>
            </div>
            <div className={styles.statsCardItem}>
              <div className={styles.statsCardIcon}>⏳</div>
              <div className={styles.statsCardInfo}>
                <span className={styles.statsCardLabel}>Remaining</span>
                <span className={styles.statsCardValue}>{quiz.questions.length - answeredCount}</span>
              </div>
            </div>
          </div>
        </main>
      </div>

      {/* Mobile Toggle Button */}
      <button
        className={styles.mobileSidebarToggle}
        onClick={() => setShowSidebar(!showSidebar)}
      >
        <span>📋</span>
        <span className={styles.mobileBadge}>{answeredCount}/{quiz.questions.length}</span>
      </button>
    </div>
  );
};

export default TakeQuizWithOTP;