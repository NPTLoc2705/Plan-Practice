import React, { useState } from 'react';
import QuizAPI from '../components/APIService/StudentQuizAPI';      
import QuizOTPAPI from '../components/APIService/QuizOTPAPI';
import styles from './TakeQuizWithOTP.module.css';

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
    const unansweredQuestions = quiz.questions.filter(
      q => !answers[q.id]
    );

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
        alert('Quiz submitted successfully!');
        setQuiz(null);
        setOtpCode('');
        setAnswers({});
      }
    } catch (error) {
      alert(error.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const answeredCount = Object.keys(answers).length;
  const totalQuestions = quiz?.questions?.length || 0;
  const progress = totalQuestions > 0 ? (answeredCount / totalQuestions) * 100 : 0;

  // OTP Entry Screen
  if (!quiz) {
    return (
      <div className={styles.otpContainer}>
        <div className={styles.otpCard}>
          <div className={styles.otpHeader}>
            <h2>Enter Quiz OTP</h2>
            <p>Enter the OTP code provided by your teacher to access the quiz</p>
          </div>

          <form onSubmit={handleGetQuiz} className={styles.otpForm}>
            <div className={styles.inputGroup}>
              <input
                type="text"
                value={otpCode}
                onChange={(e) => setOtpCode(e.target.value.toUpperCase())}
                placeholder="Enter OTP Code"
                className={styles.otpInput}
                maxLength="10"
                disabled={isValidatingOTP}
              />
            </div>

            {otpError && (
              <div className={styles.errorMessage}>
                {otpError}
              </div>
            )}

            <button
              type="submit"
              className={styles.btnPrimary}
              disabled={isValidatingOTP}
            >
              {isValidatingOTP ? 'Validating...' : 'Access Quiz'}
            </button>
          </form>

          <div className={styles.otpHelp}>
            <p>Don't have an OTP code? Contact your teacher to get one.</p>
          </div>
        </div>
      </div>
    );
  }

  // Quiz Taking Screen
  const currentQuestion = quiz.questions[currentQuestionIndex];
  const isLastQuestion = currentQuestionIndex === quiz.questions.length - 1;

  return (
    <div className={styles.takeQuizContainer}>
      <div className={styles.quizHeader}>
        <h2>{quiz.title}</h2>
        {quiz.description && <p>{quiz.description}</p>}
        
        <div className={styles.progressBar}>
          <div 
            className={styles.progressFill} 
            style={{ width: `${progress}%` }}
          />
        </div>
        
        <div className={styles.questionCounter}>
          Question {currentQuestionIndex + 1} of {totalQuestions} | 
          Answered: {answeredCount}/{totalQuestions}
        </div>
      </div>

      <div className={styles.questionContainer}>
        <h3>Question {currentQuestionIndex + 1}</h3>
        <p>{currentQuestion.content}</p>

        <div className={styles.answersList}>
          {currentQuestion.answers.map((answer) => (
            <label
              key={answer.id}
              className={styles.answerOption}
            >
              <input
                type="radio"
                name={`question-${currentQuestion.id}`}
                value={answer.id}
                checked={answers[currentQuestion.id] === answer.id}
                onChange={() => handleAnswerSelect(currentQuestion.id, answer.id)}
              />
              {answer.content}
            </label>
          ))}
        </div>
      </div>

      <div className={styles.quizNavigation}>
        <button
          onClick={goToPreviousQuestion}
          disabled={currentQuestionIndex === 0}
          className={styles.btnSecondary}
        >
          Previous
        </button>

        {!isLastQuestion ? (
          <button
            onClick={goToNextQuestion}
            className={styles.btnPrimary}
          >
            Next
          </button>
        ) : (
          <button
            onClick={handleSubmitQuiz}
            disabled={isSubmitting}
            className={styles.btnSuccess}
          >
            {isSubmitting ? 'Submitting...' : 'Submit Quiz'}
          </button>
        )}
      </div>

      <div className={styles.questionOverview}>
        <h4>Question Overview</h4>
        <div className={styles.questionDots}>
          {quiz.questions.map((question, index) => (
            <button
              key={question.id}
              onClick={() => goToQuestion(index)}
              className={`${styles.dot} ${
                index === currentQuestionIndex ? styles.active : ''
              } ${answers[question.id] ? styles.answered : ''}`}
            >
              {index + 1}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default TakeQuizWithOTP;