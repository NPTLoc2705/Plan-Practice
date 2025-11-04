import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { BookOpen, Loader2, AlertCircle, ArrowLeft, Edit, Download, Printer, FileText, Clock, History } from 'lucide-react';

const API_BASE_URL = 'https://localhost:7025/api';

const ViewLesson = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const contentRef = useRef(null);
  
  const [lessonData, setLessonData] = useState(null);
  const [documentVersions, setDocumentVersions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [loadingVersions, setLoadingVersions] = useState(false);
  const [showVersions, setShowVersions] = useState(false);

  useEffect(() => {
    fetchLessonData();
    fetchDocumentVersions();
  }, [id]);

  useEffect(() => {
    // Set the HTML content whenever lessonData changes
    if (contentRef.current && lessonData?.content) {
      console.log('Setting content:', lessonData.content.substring(0, 100));
      contentRef.current.innerHTML = lessonData.content;
    }
  }, [lessonData]);

  const fetchLessonData = async () => {
    setLoading(true);
    setError('');
    
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('Authentication required. Please log in.');
      }

      const response = await fetch(`${API_BASE_URL}/LessonPlanner/${id}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch lesson: ${response.status}`);
      }

      const result = await response.json();
      
      if (result.success && result.data) {
        setLessonData(result.data);
      } else {
        throw new Error('Invalid response format from server');
      }
    } catch (err) {
      console.error('Error fetching lesson:', err);
      setError(err.message || 'Failed to load lesson plan');
    } finally {
      setLoading(false);
    }
  };

  const fetchDocumentVersions = async () => {
    setLoadingVersions(true);
    
    try {
      const token = localStorage.getItem('token');
      if (!token) return;

      const response = await fetch(`${API_BASE_URL}/LessonPlannerDocument/lesson/${id}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        console.error('Failed to fetch document versions:', response.status);
        return;
      }

      const result = await response.json();
      
      if (result.success && result.data) {
        // Sort by creation date, newest first
        const sortedVersions = result.data.sort((a, b) => 
          new Date(b.createdAt) - new Date(a.createdAt)
        );
        setDocumentVersions(sortedVersions);
      }
    } catch (err) {
      console.error('Error fetching document versions:', err);
    } finally {
      setLoadingVersions(false);
    }
  };

  const handleDownloadVersion = async (documentId, fileName) => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        alert('Authentication required. Please log in.');
        return;
      }

      console.log('Downloading document:', documentId, fileName);

      const response = await fetch(`${API_BASE_URL}/LessonPlannerDocument/download/${documentId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      console.log('Download response status:', response.status);

      if (!response.ok) {
        const errorText = await response.text();
        console.error('Download error response:', errorText);
        
        let errorMessage = 'Failed to download document';
        try {
          const errorJson = JSON.parse(errorText);
          errorMessage = errorJson.message || errorMessage;
        } catch (e) {
          errorMessage = errorText || errorMessage;
        }
        
        throw new Error(errorMessage);
      }

      const blob = await response.blob();
      console.log('Blob size:', blob.size, 'Type:', blob.type);
      
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName || 'lesson_document.doc';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
      
      console.log('Download completed successfully');
    } catch (err) {
      console.error('Error downloading document:', err);
      alert(`Failed to download document: ${err.message}`);
    }
  };

  const handleEdit = () => {
    navigate(`/teacher/LessonPlanner/edit/${id}`);
  };

  const handleBack = () => {
    navigate('/teacher');
  };

  const handleDownload = () => {
    if (contentRef.current && lessonData) {
      const htmlContent = contentRef.current.innerHTML;
      const title = lessonData.title || 'Lesson_Plan';
      const filename = `${title.replace(/\s/g, '_')}.doc`;
      const html = `<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body><div style="padding: 20px; font-family: Calibri, sans-serif; line-height: 1.6;">${htmlContent}</div></body></html>`;
      const blob = new Blob(['\uFEFF', html], { type: 'application/msword;charset=utf-8' });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    }
  };



  // Loading state
  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
        <div className="text-center">
          <Loader2 className="w-16 h-16 text-blue-600 animate-spin mx-auto mb-4" />
          <p className="text-xl text-gray-700">Loading lesson plan...</p>
        </div>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-red-50 to-pink-100">
        <div className="bg-white p-8 rounded-lg shadow-lg max-w-md">
          <AlertCircle className="w-16 h-16 text-red-600 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-800 mb-2 text-center">Error</h2>
          <p className="text-gray-600 text-center mb-4">{error}</p>
          <div className="flex gap-3">
            <button
              onClick={fetchLessonData}
              className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition duration-200"
            >
              Retry
            </button>
            <button
              onClick={handleBack}
              className="flex-1 bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700 transition duration-200"
            >
              Go Back
            </button>
          </div>
        </div>
      </div>
    );
  }

  // No data state
  if (!lessonData) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-xl text-gray-600">No lesson data available</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 p-4 md:p-8">
      <div className="max-w-5xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div className="flex items-center gap-3">
              <button
                onClick={handleBack}
                className="p-2 hover:bg-white/50 rounded-lg transition duration-200"
                title="Back to Dashboard"
              >
                <ArrowLeft className="w-6 h-6 text-gray-700" />
              </button>
              <div>
                <h1 className="text-3xl font-bold text-gray-800 flex items-center gap-2">
                  <BookOpen className="w-8 h-8 text-blue-600" />
                  {lessonData.title}
                </h1>
                {lessonData.description && (
                  <p className="text-gray-600 mt-1">{lessonData.description}</p>
                )}
              </div>
            </div>
            
            <div className="flex gap-2 print:hidden">
              <button
                onClick={handleDownload}
                className="flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition duration-200 shadow-md"
                title="Download as .doc"
              >
                <Download className="w-4 h-4" />
                <span className="hidden sm:inline">Download</span>
              </button>
              <button
                onClick={handleEdit}
                className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition duration-200 shadow-md"
                title="Edit lesson plan"
              >
                <Edit className="w-4 h-4" />
                <span className="hidden sm:inline">Edit</span>
              </button>
            </div>
          </div>
        </div>

        {/* Metadata Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6 print:hidden">
          {lessonData.className && (
            <div className="bg-white p-4 rounded-lg shadow">
              <p className="text-sm text-gray-500 mb-1">Class</p>
              <p className="text-lg font-semibold text-gray-800">{lessonData.className}</p>
            </div>
          )}
          {lessonData.dateOfPreparation && (
            <div className="bg-white p-4 rounded-lg shadow">
              <p className="text-sm text-gray-500 mb-1">Date of Preparation</p>
              <p className="text-lg font-semibold text-gray-800">
                {new Date(lessonData.dateOfPreparation).toLocaleDateString('en-US', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric'
                })}
              </p>
            </div>
          )}
          {lessonData.dateOfTeaching && (
            <div className="bg-white p-4 rounded-lg shadow">
              <p className="text-sm text-gray-500 mb-1">Date of Teaching</p>
              <p className="text-lg font-semibold text-gray-800">
                {new Date(lessonData.dateOfTeaching).toLocaleDateString('en-US', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric'
                })}
              </p>
            </div>
          )}
        </div>

        {/* Document Version History */}
        {documentVersions.length > 0 && (
          <div className="mb-6">
            <button
              onClick={() => setShowVersions(!showVersions)}
              className="w-full bg-white p-4 rounded-lg shadow hover:shadow-md transition duration-200 flex items-center justify-between"
            >
              <div className="flex items-center gap-3">
                <History className="w-5 h-5 text-indigo-600" />
                <h3 className="text-lg font-semibold text-gray-800">
                  Document Version History ({documentVersions.length})
                </h3>
              </div>
              <span className="text-gray-500">
                {showVersions ? '▼' : '▶'}
              </span>
            </button>

            {showVersions && (
              <div className="mt-4 bg-white rounded-lg shadow overflow-hidden">
                {loadingVersions ? (
                  <div className="p-8 text-center">
                    <Loader2 className="w-8 h-8 text-blue-600 animate-spin mx-auto mb-2" />
                    <p className="text-gray-600">Loading versions...</p>
                  </div>
                ) : (
                  <div className="divide-y divide-gray-200">
                    {documentVersions.map((doc, index) => {
                      // Extract filename from filePath
                      const fileName = doc.filePath ? doc.filePath.split(/[/\\]/).pop() : `Version_${documentVersions.length - index}.doc`;
                      
                      return (
                        <div 
                          key={doc.id} 
                          className="p-4 hover:bg-gray-50 transition duration-150"
                        >
                          <div className="flex items-start justify-between gap-4">
                            <div className="flex-1">
                              <div className="flex items-center gap-2 mb-2">
                                <FileText className="w-4 h-4 text-blue-600" />
                                <h4 className="font-semibold text-gray-800">
                                  {fileName}
                                </h4>
                                {index === 0 && (
                                  <span className="px-2 py-1 text-xs font-semibold bg-green-100 text-green-800 rounded">
                                    Latest
                                  </span>
                                )}
                              </div>
                              
                              <div className="flex flex-wrap gap-4 text-sm text-gray-600">
                                <div className="flex items-center gap-1">
                                  <Clock className="w-4 h-4" />
                                  <span>
                                    Created: {new Date(doc.createdAt).toLocaleString('en-US', {
                                      year: 'numeric',
                                      month: 'short',
                                      day: 'numeric',
                                      hour: '2-digit',
                                      minute: '2-digit'
                                    })}
                                  </span>
                                </div>
                                
                                <div className="flex items-center gap-1">
                                  <span>Document ID: {doc.id}</span>
                                </div>
                              </div>
                            </div>

                            <button
                              onClick={() => handleDownloadVersion(doc.id, fileName)}
                              className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition duration-200 shadow-sm"
                              title="Download this version"
                            >
                              <Download className="w-4 h-4" />
                              <span className="hidden sm:inline">Download</span>
                            </button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            )}
          </div>
        )}

        {/* Lesson Content */}
        <div className="bg-white rounded-xl shadow-lg overflow-hidden">
          <div 
            ref={contentRef}
            className="p-8 max-w-none overflow-auto"
            style={{ 
              wordWrap: 'break-word', 
              overflowWrap: 'break-word',
              fontFamily: 'Arial, sans-serif',
              minHeight: '400px',
              lineHeight: '1.6'
            }}
          >
            {/* Content will be injected here via innerHTML */}
            {!lessonData?.content && (
              <div className="text-center text-gray-400 py-12">
                <BookOpen className="w-16 h-16 mx-auto mb-4 opacity-50" />
                <p className="text-lg">No content available for this lesson</p>
              </div>
            )}
          </div>
        </div>

        {/* Footer Info */}
        <div className="mt-6 text-center text-sm text-gray-500 print:hidden">
          <p>Created: {new Date(lessonData.createdAt).toLocaleString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
          })}</p>
          {lessonData.updatedAt && (
            <p>Last Updated: {new Date(lessonData.updatedAt).toLocaleString('en-US', {
              year: 'numeric',
              month: 'long',
              day: 'numeric',
              hour: '2-digit',
              minute: '2-digit'
            })}</p>
          )}
        </div>
      </div>

      {/* Print Styles */}
      <style>{`
        @media print {
          body {
            background: white !important;
          }
          .print\\:hidden {
            display: none !important;
          }
          .bg-gradient-to-br {
            background: white !important;
          }
        }
      `}</style>
    </div>
  );
};

export default ViewLesson;
