import React, { useState, useRef, useEffect } from 'react';
import {
  BookOpen,
  Send,
  Loader2,
  Bold,
  Italic,
  Underline,
  List,
  ListOrdered,
  Save,
  Copy,
  Download,
  FileText,
  ChevronDown,
  Image as ImageIcon,
  Strikethrough,
  Code,
  Quote,
  Heading1,
  Heading2,
  Heading3,
  AlignLeft,
  AlignCenter,
  AlignRight,
  AlignJustify,
  Type,
  Highlighter,
  Palette,
} from 'lucide-react';

// API Base URL
const API_BASE_URL = 'https://localhost:7025/api/Lesson';

// Utility function to get JWT token
const getAuthToken = () => {
  return localStorage.getItem('token');
};

// Utility function to make authenticated API calls
const apiFetch = async (url, options = {}) => {
  const token = getAuthToken();
  const headers = {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` }),
    ...options.headers,
  };

  const response = await fetch(url, {
    ...options,
    headers,
  });

  const contentType = response.headers.get('content-type');
  if (!contentType || !contentType.includes('application/json')) {
    const text = await response.text();
    throw new Error(`Server returned a non-JSON response: ${text.slice(0, 50)}...`);
  }

  const result = await response.json();
  if (!response.ok) {
    throw new Error(result.message || `API request failed with status ${response.status}`);
  }
  if (result.success === false) {
    throw new Error(result.message || 'Request failed');
  }

  return result;
};

// Helper to convert structured lesson response to HTML string for the editor
const lessonToHtml = (lesson) => {
  if (!lesson) return '';

  const formatSection = (title, content) => {
    if (!content) return '';
    let formattedContent = content.split('\n').map(line => {
      if (line.trim().startsWith('•')) {
        return `<li>${line.replace('•', '').trim()}</li>`;
      }
      return `<p>${line}</p>`;
    }).join('');

    if (formattedContent.includes('<li>')) {
      formattedContent = `<ul>${formattedContent.replace(/<\/p><p>/g, '').replace(/<p><li>/g, '<li>').replace(/<\/li><\/p>/g, '</li>')}</ul>`;
    }

    return `
      <h3 style="font-weight: 700; margin-top: 1.5rem; margin-bottom: 0.5rem; font-size: 1.125rem;">${title}</h3>
      ${formattedContent}
    `;
  };

  const titleHtml = `<h2 style="font-weight: 700; font-size: 1.5rem; color: #1f2937;">${lesson.title}</h2>`;
  const descriptionHtml = lesson.description ? formatSection('Description', lesson.description) : '';
  const contentHtml = formatSection('Content', lesson.content);

  return titleHtml + descriptionHtml + contentHtml;
};

// Main App Component
export default function App() {
  const [gradeLevel, setGradeLevel] = useState('');
  const [lessonTitle, setLessonTitle] = useState('');
  const [lessonDescription, setLessonDescription] = useState('');
  const [isGenerating, setIsGenerating] = useState(false);
  const [message, setMessage] = useState('');
  const [lesson, setLesson] = useState(null);
  const [showFontSize, setShowFontSize] = useState(false);
  const [showFontFamily, setShowFontFamily] = useState(false);
  const [showHighlight, setShowHighlight] = useState(false);
  const [showTextColor, setShowTextColor] = useState(false);

  const contentRef = useRef(null);
  const fileInputRef = useRef(null);
  const tableRowsRef = useRef(null);
  const tableColsRef = useRef(null);

  useEffect(() => {
    if (contentRef.current && lesson) {
      contentRef.current.innerHTML = lessonToHtml(lesson);
    }
  }, [lesson]);

  const handleGeneratePlan = async () => {
    if (isGenerating) return;

    setMessage('');
    setIsGenerating(true);

    const request = {
      title: lessonTitle,
      description: lessonDescription,
      gradeLevel: parseInt(gradeLevel) || 1,
      content: '',
    };

    try {
      setMessage('AI generation is not implemented in this version.');
    } catch (error) {
      console.error('Generation Error:', error);
      setMessage('An unexpected error occurred during generation.');
    } finally {
      setIsGenerating(false);
    }
  };

  const handleFormat = (command, value = null) => {
    document.execCommand(command, false, value);
    if (contentRef.current) {
      contentRef.current.focus();
    }
  };

  const handleImageUpload = (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (event) => {
        const img = document.createElement('img');
        img.src = event.target.result;
        img.style.maxWidth = '100%';
        img.style.height = 'auto';
        img.style.marginTop = '1rem';
        img.style.marginBottom = '1rem';
        img.style.borderRadius = '0.5rem';

        if (contentRef.current) {
          contentRef.current.focus();
          const range = window.getSelection().getRangeAt(0);
          range.insertNode(img);
          contentRef.current.focus();
        }
      };
      reader.readAsDataURL(file);
    }
  };

  const insertTable = () => {
    const rows = parseInt(tableRowsRef.current?.value || 3);
    const cols = parseInt(tableColsRef.current?.value || 3);

    if (rows > 0 && cols > 0) {
      let tableHtml = '<table style="border-collapse: collapse; width: 100%; margin: 1rem 0;"><tbody>';
      for (let i = 0; i < rows; i++) {
        tableHtml += '<tr>';
        for (let j = 0; j < cols; j++) {
          tableHtml += `<td style="border: 1px solid #ccc; padding: 8px; background-color: ${i === 0 ? '#f0f0f0' : 'white'};"><br></td>`;
        }
        tableHtml += '</tr>';
      }
      tableHtml += '</tbody></table>';

      handleFormat('insertHTML', tableHtml);
      setShowFontSize(false);
    }
  };

  const handleCopyText = () => {
    if (contentRef.current) {
      const range = document.createRange();
      range.selectNode(contentRef.current);
      window.getSelection().removeAllRanges();
      window.getSelection().addRange(range);

      try {
        document.execCommand('copy');
        setMessage('Lesson copied to clipboard!');
      } catch (err) {
        console.error('Failed to copy text: ', err);
        setMessage('Failed to copy text.');
      }
      window.getSelection().removeAllRanges();
    }
  };

  const handleDownloadDocx = () => {
    if (contentRef.current) {
      const htmlContent = contentRef.current.innerHTML;
      const title = lessonTitle || 'Lesson_Plan';
      const filename = `${title.replace(/\s/g, '_')}.doc`;

      const html = `
        <!DOCTYPE html>
        <html>
        <head><meta charset="UTF-8"></head>
        <body>
          <div style="padding: 20px; font-family: Calibri, sans-serif; line-height: 1.6;">
            ${htmlContent}
          </div>
        </body>
        </html>
      `;

      const blob = new Blob([html], {
        type: 'application/msword;charset=utf-8'
      });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
      setMessage('Lesson plan downloaded as .doc!');
    }
  };

  const handleExportToDocs = () => {
    setMessage('Export to Google Docs is not implemented in this version.');
  };

  const handleSaveLesson = async () => {
    if (!getAuthToken()) {
      setMessage('Please log in to save a lesson.');
      return;
    }

    const content = contentRef.current?.innerHTML || '';
    const saveRequest = {
      title: lessonTitle,
      description: lessonDescription,
      gradeLevel: parseInt(gradeLevel) || 1,
      content,
    };

    setMessage('Saving lesson...');

    try {
      const result = await apiFetch(API_BASE_URL, {
        method: 'POST',
        body: JSON.stringify(saveRequest),
      });

      if (result.success) {
        setLesson(result.data);
        setMessage(`Lesson saved successfully! ID: ${result.data.id}`);
      }
    } catch (error) {
      console.error('Save Error:', error);
      setMessage('Failed to save lesson: ' + error.message);
    }
  };

  const RichTextToolbar = () => (
    <div className="flex flex-wrap items-center space-y-2 p-3 border-b bg-gradient-to-r from-gray-50 to-gray-100 border-gray-200 rounded-t-xl sticky top-0 z-10">
      {/* Row 1: Basic Formatting */}
      <div className="flex items-center space-x-1 border-r pr-3 flex-wrap gap-1">
        {[
          { Icon: Bold, cmd: 'bold', title: 'Bold' },
          { Icon: Italic, cmd: 'italic', title: 'Italic' },
          { Icon: Underline, cmd: 'underline', title: 'Underline' },
          { Icon: Strikethrough, cmd: 'strikethrough', title: 'Strikethrough' },
        ].map(({ Icon, cmd, title }) => (
          <button
            key={cmd}
            className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition"
            onClick={() => handleFormat(cmd)}
            title={title}
          >
            <Icon className="w-4 h-4" />
          </button>
        ))}
      </div>

      {/* Row 2: List Formatting */}
      <div className="flex space-x-1 border-r pr-3">
        {[
          { Icon: List, cmd: 'insertUnorderedList', title: 'Bullet List' },
          { Icon: ListOrdered, cmd: 'insertOrderedList', title: 'Numbered List' },
        ].map(({ Icon, cmd, title }) => (
          <button
            key={cmd}
            className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition"
            onClick={() => handleFormat(cmd)}
            title={title}
          >
            <Icon className="w-4 h-4" />
          </button>
        ))}
      </div>

      {/* Row 3: Headings */}
      <div className="flex space-x-1 border-r pr-3">
        {[
          { Icon: Heading1, cmd: 'formatBlock', value: 'h1', title: 'Heading 1' },
          { Icon: Heading2, cmd: 'formatBlock', value: 'h2', title: 'Heading 2' },
          { Icon: Heading3, cmd: 'formatBlock', value: 'h3', title: 'Heading 3' },
        ].map(({ Icon, cmd, value, title }) => (
          <button
            key={value}
            className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition"
            onClick={() => handleFormat(cmd, value)}
            title={title}
          >
            <Icon className="w-4 h-4" />
          </button>
        ))}
      </div>

      {/* Row 4: Alignment */}
      <div className="flex space-x-1 border-r pr-3">
        {[
          { Icon: AlignLeft, cmd: 'justifyLeft', title: 'Align Left' },
          { Icon: AlignCenter, cmd: 'justifyCenter', title: 'Align Center' },
          { Icon: AlignRight, cmd: 'justifyRight', title: 'Align Right' },
          { Icon: AlignJustify, cmd: 'justifyFull', title: 'Justify' },
        ].map(({ Icon, cmd, title }) => (
          <button
            key={cmd}
            className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition"
            onClick={() => handleFormat(cmd)}
            title={title}
          >
            <Icon className="w-4 h-4" />
          </button>
        ))}
      </div>

      {/* Row 5: Insert Elements */}
      <div className="flex space-x-1 border-r pr-3">
        <button
          className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition"
          onClick={() => fileInputRef.current?.click()}
          title="Insert Image"
        >
          <ImageIcon className="w-4 h-4" />
        </button>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          onChange={handleImageUpload}
          className="hidden"
        />
        <button
          className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition"
          onClick={() => handleFormat('createLink', prompt('Enter URL:'))}
          title="Insert Link"
        >
          <Type className="w-4 h-4" />
        </button>
        <button
          className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition relative"
          onClick={() => setShowFontSize(!showFontSize)}
          title="Insert Table"
        >
          <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
            <path d="M3 2h18a1 1 0 011 1v18a1 1 0 01-1 1H3a1 1 0 01-1-1V3a1 1 0 011-1zm1 2v16h16V4H4zm1 1h2v2H5V5zm4 0h2v2H9V5zm4 0h2v2h-2V5zm4 0h2v2h-2V5zM5 9h14v2H5V9zm0 4h14v2H5v-2z"/>
          </svg>
          {showFontSize && (
            <div className="absolute top-full mt-1 bg-white border border-gray-300 rounded shadow-lg p-3 z-20 w-max">
              <div className="flex gap-2 mb-2">
                <input
                  ref={tableRowsRef}
                  type="number"
                  placeholder="Rows"
                  defaultValue="3"
                  min="1"
                  max="20"
                  className="w-16 p-1 border border-gray-300 rounded"
                />
                <input
                  ref={tableColsRef}
                  type="number"
                  placeholder="Cols"
                  defaultValue="3"
                  min="1"
                  max="20"
                  className="w-16 p-1 border border-gray-300 rounded"
                />
              </div>
              <button
                onClick={insertTable}
                className="w-full px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700 text-sm"
              >
                Insert Table
              </button>
            </div>
          )}
        </button>
      </div>

      {/* Row 6: Colors & Effects */}
      <div className="flex space-x-1 border-r pr-3">
        <button
          className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition relative"
          onClick={() => setShowTextColor(!showTextColor)}
          title="Text Color"
        >
          <Palette className="w-4 h-4" />
          {showTextColor && (
            <div className="absolute top-full mt-1 bg-white border border-gray-300 rounded shadow-lg p-2 z-20 grid grid-cols-5 gap-1">
              {['#000000', '#FF0000', '#00B050', '#0070C0', '#FFC000', '#7030A0', '#00B0F0', '#FF6B35', '#004E89', '#9D4EDD'].map((color) => (
                <button
                  key={color}
                  onClick={() => handleFormat('foreColor', color)}
                  className="w-6 h-6 rounded border-2 border-gray-300 hover:border-gray-600"
                  style={{ backgroundColor: color }}
                  title={color}
                />
              ))}
            </div>
          )}
        </button>
        <button
          className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition relative"
          onClick={() => setShowHighlight(!showHighlight)}
          title="Highlight"
        >
          <Highlighter className="w-4 h-4" />
          {showHighlight && (
            <div className="absolute top-full mt-1 bg-white border border-gray-300 rounded shadow-lg p-2 z-20 grid grid-cols-5 gap-1">
              {['#FFFF00', '#FFC000', '#FF6B35', '#00B050', '#92D050', '#00B0F0', '#E0FFFF', '#FFE699', '#FFD699', '#F4B084'].map((color) => (
                <button
                  key={color}
                  onClick={() => handleFormat('backColor', color)}
                  className="w-6 h-6 rounded border-2 border-gray-300 hover:border-gray-600"
                  style={{ backgroundColor: color }}
                  title={color}
                />
              ))}
            </div>
          )}
        </button>
      </div>

      {/* Row 7: Additional */}
      <div className="flex space-x-1 border-r pr-3">
        {[
          { Icon: Code, cmd: 'formatBlock', value: 'pre', title: 'Code Block' },
          { Icon: Quote, cmd: 'formatBlock', value: 'blockquote', title: 'Quote' },
        ].map(({ Icon, cmd, value, title }) => (
          <button
            key={value}
            className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition"
            onClick={() => handleFormat(cmd, value)}
            title={title}
          >
            <Icon className="w-4 h-4" />
          </button>
        ))}
      </div>

      {/* Save Button */}
      <button
        className="ml-auto flex items-center space-x-1 px-3 py-1.5 text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition"
        onClick={handleSaveLesson}
        disabled={isGenerating}
      >
        <Save className="w-4 h-4" />
        <span className="text-sm font-medium">Save Lesson</span>
      </button>
    </div>
  );

  return (
    <div className="min-h-screen bg-gray-50 p-4 sm:p-8 font-['Inter']">
      <header className="mb-8">
        <h1 className="text-3xl font-extrabold text-gray-900 flex items-center">
          <BookOpen className="w-8 h-8 mr-3 text-red-600" />
          Create Lesson Plan
        </h1>
        <p className="text-gray-500 mt-1">
          Create a new lesson plan with advanced formatting
        </p>
      </header>

      {message && (
        <div className="mb-4 p-3 bg-indigo-100 text-indigo-800 rounded-lg shadow-sm text-sm font-medium">
          {message}
        </div>
      )}

      <div className="flex flex-col lg:flex-row gap-8">
        {/* LEFT COLUMN: Input Form */}
        <div className="lg:w-1/3 p-6 bg-white rounded-xl shadow-2xl h-fit sticky top-4">
          <div className="space-y-6">
            {/* Grade Level Dropdown */}
            <div>
              <label htmlFor="grade-level" className="block text-lg font-bold text-gray-700 mb-2">
                Grade Level
              </label>
              <select
                id="grade-level"
                value={gradeLevel}
                onChange={(e) => setGradeLevel(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-lg shadow-sm focus:ring-red-500 focus:border-red-500 transition duration-150 ease-in-out bg-white appearance-none"
              >
                <option value="">Select Grade</option>
                {[...Array(12).keys()].map(i => (
                  <option key={i + 1} value={i + 1}>{i + 1}th Grade</option>
                ))}
                <option value="0">Kindergarten</option>
              </select>
            </div>

            {/* Lesson Title Input */}
            <div>
              <label htmlFor="lesson-title" className="block text-lg font-bold text-gray-700 mb-2">
                Lesson Title
              </label>
              <input
                id="lesson-title"
                type="text"
                placeholder="Enter lesson title"
                value={lessonTitle}
                onChange={(e) => setLessonTitle(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-lg shadow-sm focus:ring-red-500 focus:border-red-500 transition"
              />
              <p className="mt-1 text-sm text-gray-400">{lessonTitle.length} characters</p>
            </div>

            {/* Lesson Description Textarea */}
            <div>
              <label htmlFor="lesson-description" className="block text-lg font-bold text-gray-700 mb-2">
                Lesson Description
              </label>
              <textarea
                id="lesson-description"
                placeholder="Enter lesson description"
                rows="6"
                value={lessonDescription}
                onChange={(e) => setLessonDescription(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-lg shadow-sm focus:ring-red-500 focus:border-red-500 transition resize-none"
              />
              <p className="mt-1 text-sm text-gray-400">{lessonDescription.split(/\s+/).filter(Boolean).length} words</p>
            </div>

            {/* Generate Button */}
            <button
              onClick={handleGeneratePlan}
              disabled={isGenerating || !lessonTitle.trim() || !lessonDescription.trim()}
              className={`w-full py-3 mt-4 text-white font-semibold rounded-lg shadow-md transition duration-300 ease-in-out flex justify-center items-center ${
                isGenerating ? 'bg-red-400 cursor-not-allowed' : 'bg-red-600 hover:bg-red-700 active:bg-red-800'
              }`}
            >
              {isGenerating ? (
                <>
                  <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                  Generating...
                </>
              ) : (
                <>
                  <Send className="w-5 h-5 mr-2" />
                  Generate (AI)
                </>
              )}
            </button>
          </div>
        </div>

        {/* RIGHT COLUMN: Lesson Plan Editor/Output */}
        <div className="lg:w-2/3 flex flex-col">
          <div className="bg-white rounded-xl shadow-2xl overflow-hidden">
            <RichTextToolbar />
            <div
              ref={contentRef}
              contentEditable={true}
              suppressContentEditableWarning={true}
              className="p-6 h-[70vh] overflow-y-auto outline-none prose prose-sm max-w-none text-gray-800 focus:ring-2 focus:ring-red-500"
              style={{
                wordWrap: 'break-word',
                overflowWrap: 'break-word',
              }}
            >
              {/* Content is managed by React state and innerHTML */}
            </div>
          </div>

          {/* Action Buttons at the Bottom Right */}
          <div className="mt-6 flex justify-end space-x-4 flex-wrap gap-2">
            <button
              onClick={handleCopyText}
              className="flex items-center space-x-2 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition shadow-md"
            >
              <Copy className="w-4 h-4" />
              <span>Copy text</span>
            </button>
            <button
              onClick={handleDownloadDocx}
              className="flex items-center space-x-2 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition shadow-md"
            >
              <Download className="w-4 h-4" />
              <span>Download .doc</span>
            </button>
            <button
              onClick={handleExportToDocs}
              className="flex items-center space-x-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition shadow-md"
            >
              <FileText className="w-4 h-4" />
              <span>Export to Docs</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}