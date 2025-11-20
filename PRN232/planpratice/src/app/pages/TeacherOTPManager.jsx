import React, { useState, useEffect } from 'react';
import QuizAPI from '../components/APIService/QuizAPI';
import QuizOTPAPI from '../components/APIService/QuizOTPAPI';
import styles from './TeacherOTPManager.module.css';

const TeacherOTPManager = () => {
  const [activeTab, setActiveTab] = useState('generate');
  
  // Generate OTP states
  const [quizzes, setQuizzes] = useState([]);
  const [selectedQuizId, setSelectedQuizId] = useState('');
  const [otpSettings, setOtpSettings] = useState({
    expiryMinutes: 60,
    maxUsage: '',
    allowMultipleAttempts: true
  });
  const [isGenerating, setIsGenerating] = useState(false);
  const [generateSuccess, setGenerateSuccess] = useState(null);
  const [generateError, setGenerateError] = useState('');
const [showRegeneratedOTP, setShowRegeneratedOTP] = useState(null);

  // Manage OTP states
  const [myOTPs, setMyOTPs] = useState([]);
  const [isLoadingOTPs, setIsLoadingOTPs] = useState(false);
  const [accessLogs, setAccessLogs] = useState([]);
  const [showLogsModal, setShowLogsModal] = useState(false);

  useEffect(() => {
    fetchQuizzes();
  }, []);

  useEffect(() => {
    if (activeTab === 'manage') {
      fetchMyOTPs();
    }
  }, [activeTab]);

  const fetchQuizzes = async () => {
    try {
      const response = await QuizAPI.getTeacherQuizzes();
      if (response.success) {
        setQuizzes(response.data);
      }
    } catch (error) {
      console.error('Error fetching quizzes:', error);
    }
  };

  const fetchMyOTPs = async () => {
    setIsLoadingOTPs(true);
    try {
      const response = await QuizOTPAPI.getMyOTPs();
      if (response.success) {
        setMyOTPs(response.data);
      }
    } catch (error) {
      console.error('Error fetching OTPs:', error);
    } finally {
      setIsLoadingOTPs(false);
    }
  };

  const handleGenerateOTP = async (e) => {
    e.preventDefault();
    
    if (!selectedQuizId) {
      setGenerateError('Please select a quiz');
      return;
    }

    setIsGenerating(true);
    setGenerateError('');
    setGenerateSuccess(null);

    try {
      const otpData = QuizOTPAPI.formatOTPData(
        selectedQuizId,
        otpSettings.expiryMinutes,
        otpSettings.maxUsage,
        otpSettings.allowMultipleAttempts
      );

      const response = await QuizOTPAPI.generateOTP(otpData);

      if (response.success) {
        setGenerateSuccess(response.data);
        setSelectedQuizId('');
        setOtpSettings({
          expiryMinutes: 60,
          maxUsage: '',
          allowMultipleAttempts: true
        });
      }
    } catch (error) {
      setGenerateError(error.message);
    } finally {
      setIsGenerating(false);
    }
  };

  const handleRevokeOTP = async (otpId) => {
    if (!window.confirm('Are you sure you want to revoke this OTP?')) {
      return;
    }

    try {
      const response = await QuizOTPAPI.revokeOTP(otpId);
      if (response.success) {
        alert('OTP revoked successfully');
        fetchMyOTPs();
      }
    } catch (error) {
      alert(error.message);
    }
  };

  const handleExtendOTP = async (otpId) => {
    const minutes = prompt('Enter additional minutes:', '30');
    if (!minutes) return;

    try {
      const response = await QuizOTPAPI.extendOTP(otpId, parseInt(minutes));
      if (response.success) {
        alert(`OTP extended by ${minutes} minutes`);
        fetchMyOTPs();
      }
    } catch (error) {
      alert(error.message);
    }
  };

  // Update the handleRegenerateOTP function to properly show the new OTP
const handleRegenerateOTP = async (otpId) => {
    if (!window.confirm('Generate a new OTP with the same settings?')) {
        return;
    }

    try {
        const response = await QuizOTPAPI.regenerateOTP(otpId);
        if (response.success) {
            // Show the new OTP in a modal
            setShowRegeneratedOTP(response.data);
            
            // Copy to clipboard automatically
            await QuizOTPAPI.copyToClipboard(response.data.otpCode || response.data.OTPCode);
            
            // Refresh the list
            fetchMyOTPs();
        }
    } catch (error) {
        alert(error.message);
    }
};

// Alternative: If you want to keep it in the manage tab, update the state differently
const handleRegenerateOTPAlternative = async (otpId) => {
    if (!window.confirm('Generate a new OTP with the same settings?')) {
        return;
    }

    try {
        const response = await QuizOTPAPI.regenerateOTP(otpId);
        if (response.success) {
            // Create a modal or a better UI to show the new OTP
            const newOTPCode = response.data.otpCode || response.data.OTPCode;
            
            // Show in a more user-friendly way
            const message = `New OTP Generated!\n\nOTP Code: ${newOTPCode}\n\nThe OTP has been copied to your clipboard.`;
            
            // Copy to clipboard
            await QuizOTPAPI.copyToClipboard(newOTPCode);
            
            alert(message);
            
            // Refresh the list to show the new OTP
            await fetchMyOTPs();
        }
    } catch (error) {
        alert(error.message);
    }
};

  const handleViewLogs = async (otpId) => {
    try {
      const response = await QuizOTPAPI.getOTPAccessLogs(otpId);
      if (response.success) {
        setAccessLogs(response.data);
        setShowLogsModal(true);
      }
    } catch (error) {
      alert(error.message);
    }
  };

  const copyToClipboard = async (text) => {
    const success = await QuizOTPAPI.copyToClipboard(text);
    if (success) {
      alert('OTP copied to clipboard!');
    } else {
      alert('Failed to copy OTP');
    }
  };

  const formatDate = (dateString) => QuizOTPAPI.formatDate(dateString);
  const isExpired = (expiresAt) => QuizOTPAPI.isOTPExpired(expiresAt);

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>OTP Management</h1>
        <p>Generate and manage quiz access codes</p>
      </div>

      {/* Tabs */}
      <div className={styles.tabs}>
        <button
          className={`${styles.tab} ${activeTab === 'generate' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('generate')}
        >
          Generate OTP
        </button>
        <button
          className={`${styles.tab} ${activeTab === 'manage' ? styles.activeTab : ''}`}
          onClick={() => setActiveTab('manage')}
        >
          Manage OTPs
        </button>
      </div>

      {/* Generate OTP Tab */}
      {activeTab === 'generate' && (
        <div className={styles.generateSection}>
          <div className={styles.card}>
            <h2>Generate New OTP</h2>
            <p className={styles.subtitle}>
              Create a secure access code for students to take your quiz
            </p>

            <form onSubmit={handleGenerateOTP} className={styles.form}>
              <div className={styles.formGroup}>
                <label>Select Quiz *</label>
                <select
                  value={selectedQuizId}
                  onChange={(e) => setSelectedQuizId(e.target.value)}
                  className={styles.select}
                  required
                >
                  <option value="">Choose a quiz...</option>
                  {quizzes.map((quiz) => (
    <option key={quiz.id} value={quiz.id}>
        {quiz.title} ({quiz.totalQuestions} questions)
    </option>
))}
                </select>
              </div>

              <div className={styles.formGroup}>
                <label>Expiry Time (minutes) *</label>
                <input
                  type="number"
                  value={otpSettings.expiryMinutes}
                  onChange={(e) => setOtpSettings({
                    ...otpSettings,
                    expiryMinutes: e.target.value
                  })}
                  className={styles.input}
                  min="1"
                  required
                />
                <small className={styles.hint}>
                  OTP will expire after this duration
                </small>
              </div>

              <div className={styles.formGroup}>
                <label>Maximum Usage (optional)</label>
                <input
                  type="number"
                  value={otpSettings.maxUsage}
                  onChange={(e) => setOtpSettings({
                    ...otpSettings,
                    maxUsage: e.target.value
                  })}
                  className={styles.input}
                  min="1"
                  placeholder="Unlimited"
                />
                <small className={styles.hint}>
                  Leave empty for unlimited uses
                </small>
              </div>

              <div className={styles.formGroup}>
                <label className={styles.checkboxLabel}>
                  <input
                    type="checkbox"
                    checked={otpSettings.allowMultipleAttempts}
                    onChange={(e) => setOtpSettings({
                      ...otpSettings,
                      allowMultipleAttempts: e.target.checked
                    })}
                    className={styles.checkbox}
                  />
                  Allow students to retake quiz with same OTP
                </label>
              </div>

              {generateError && (
                <div className={styles.errorMessage}>
                  {generateError}
                </div>
              )}

              <button
                type="submit"
                className={styles.btnPrimary}
                disabled={isGenerating}
              >
                {isGenerating ? 'Generating...' : 'Generate OTP'}
              </button>
            </form>

            {generateSuccess && (
              <div className={styles.successCard}>
                <h3>✓ OTP Generated Successfully!</h3>
                <div className={styles.otpDisplay}>
                  <span className={styles.otpCode}>
                    {generateSuccess.otpCode}
                  </span>
                  <button
                    onClick={() => copyToClipboard(generateSuccess.otpCode)}
                    className={styles.btnCopy}
                  >
                    Copy
                  </button>
                </div>
                <div className={styles.otpDetails}>
                  <p><strong>Quiz:</strong> {generateSuccess.quizTitle}</p>
                  <p><strong>Expires:</strong> {formatDate(generateSuccess.expiresAt)}</p>
                  {generateSuccess.maxUsage && (
                    <p><strong>Max Usage:</strong> {generateSuccess.maxUsage}</p>
                  )}
                </div>
                
              </div>
            )}
          </div>
        </div>
      )}

      {/* Manage OTPs Tab */}
      {activeTab === 'manage' && (
        <div className={styles.manageSection}>
          {isLoadingOTPs ? (
            <div className={styles.loading}>Loading OTPs...</div>
          ) : myOTPs.length === 0 ? (
            <div className={styles.emptyState}>
              <p>No OTPs generated yet</p>
              <button
                onClick={() => setActiveTab('generate')}
                className={styles.btnPrimary}
              >
                Generate Your First OTP
              </button>
            </div>
          ) : (
            <div className={styles.otpGrid}>
              {myOTPs.map((otp) => (
                <div key={otp.id} className={styles.otpCard}>
                  <div className={styles.otpCardHeader}>
                    <h3>{otp.quizTitle}</h3>
                    <span className={`${styles.badge} ${
                      !otp.isActive ? styles.badgeInactive :
                      isExpired(otp.expiresAt) ? styles.badgeExpired :
                      styles.badgeActive
                    }`}>
                      {!otp.isActive ? 'Revoked' :
                       isExpired(otp.expiresAt) ? 'Expired' : 'Active'}
                    </span>
                  </div>

                  <div className={styles.otpCodeDisplay}>
                    <span className={styles.codeLabel}>OTP Code:</span>
                    <span className={styles.code}>{otp.otpCode}</span>
                    <button
                      onClick={() => copyToClipboard(otp.otpCode)}
                      className={styles.btnIconCopy}
                      title="Copy OTP"
                    >
                      📋
                    </button>
                  </div>

                  <div className={styles.otpInfo}>
                    <div className={styles.infoRow}>
                      <span className={styles.infoLabel}>Created:</span>
                      <span>{formatDate(otp.createdAt)}</span>
                    </div>
                    <div className={styles.infoRow}>
                      <span className={styles.infoLabel}>Expires:</span>
                      <span>{formatDate(otp.expiresAt)}</span>
                    </div>
                    <div className={styles.infoRow}>
                      <span className={styles.infoLabel}>Usage:</span>
                      <span>
                        {otp.usageCount} / {otp.maxUsage || '∞'}
                      </span>
                    </div>
                  </div>

                  <div className={styles.otpActions}>
                    <button
                      onClick={() => handleViewLogs(otp.id)}
                      className={styles.btnAction}
                      title="View Access Logs"
                    >
                      📊 Logs
                    </button>
                    {otp.isActive && !isExpired(otp.expiresAt) && (
                      <>
                        <button
                          onClick={() => handleExtendOTP(otp.id)}
                          className={styles.btnAction}
                          title="Extend Expiry"
                        >
                          ⏱️ Extend
                        </button>
                        <button
                          onClick={() => handleRevokeOTP(otp.id)}
                          className={styles.btnActionDanger}
                          title="Revoke OTP"
                        >
                          🚫 Revoke
                        </button>
                      </>
                    )}
                    <button
                      onClick={() => handleRegenerateOTP(otp.id)}
                      className={styles.btnAction}
                      title="Regenerate"
                    >
                      🔄 Regenerate
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Access Logs Modal */}
      {showLogsModal && (
        <div className={styles.modal} onClick={() => setShowLogsModal(false)}>
          <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
              <h2>Access Logs</h2>
              <button
                onClick={() => setShowLogsModal(false)}
                className={styles.btnClose}
              >
                ✕
              </button>
            </div>
            <div className={styles.modalBody}>
              {accessLogs.length === 0 ? (
                <p className={styles.emptyLogs}>No access logs yet</p>
              ) : (
                <div className={styles.logsTable}>
                  <table>
                    <thead>
                      <tr>
                        <th>Student</th>
                        <th>Accessed At</th>
                      </tr>
                    </thead>
                    <tbody>
                      {accessLogs.map((log, index) => (
                        <tr key={index}>
                          <td>{log.studentName || `Student #${log.studentId}`}</td>
                          <td>{formatDate(log.accessedAt)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {showRegeneratedOTP && (
    <div className={styles.modal} onClick={() => setShowRegeneratedOTP(null)}>
        <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
            <div className={styles.modalHeader}>
                <h2>New OTP Generated</h2>
                <button
                    onClick={() => setShowRegeneratedOTP(null)}
                    className={styles.btnClose}
                >
                    ✕
                </button>
            </div>
            <div className={styles.modalBody}>
                <div className={styles.successCard}>
                    <div className={styles.otpDisplay}>
                        <span className={styles.otpCode}>
                            {showRegeneratedOTP.otpCode || showRegeneratedOTP.OTPCode}
                        </span>
                        <button
                            onClick={() => copyToClipboard(showRegeneratedOTP.otpCode || showRegeneratedOTP.OTPCode)}
                            className={styles.btnCopy}
                        >
                            Copy
                        </button>
                    </div>
                    <p className={styles.successMessage}>
                        ✓ OTP has been copied to clipboard
                    </p>
                    <div className={styles.otpDetails}>
                        <p><strong>Quiz:</strong> {showRegeneratedOTP.quizTitle || showRegeneratedOTP.QuizTitle}</p>
                        <p><strong>Expires:</strong> {formatDate(showRegeneratedOTP.expiresAt || showRegeneratedOTP.ExpiresAt)}</p>
                        {(showRegeneratedOTP.maxUsage || showRegeneratedOTP.MaxUsage) && (
                            <p><strong>Max Usage:</strong> {showRegeneratedOTP.maxUsage || showRegeneratedOTP.MaxUsage}</p>
                        )}
                    </div>
                </div>
            </div>
        </div>
    </div>
)}
    </div>
  );
};

export default TeacherOTPManager;