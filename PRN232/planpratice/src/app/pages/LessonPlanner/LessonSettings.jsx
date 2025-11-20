import React, { useState, useEffect } from 'react';
import {
  Settings,
  Plus,
  Edit2,
  Trash2,
  Save,
  X,
  BookOpen,
  Users,
  Target,
  Brain,
  Heart,
  Book,
  MessageSquare,
  Lightbulb,
  Activity,
  Layers,
  CheckCircle2,
  AlertCircle,
  Loader2
} from 'lucide-react';



const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

// API Helper Functions
const getAuthToken = () => {
  const token = localStorage.getItem('token');
  if (!token) {
    throw new Error('Authentication required. Please log in.');
  }
  return token;
};

const fetchFromApi = async (endpoint, token) => {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    headers: { 'Authorization': `Bearer ${token}` },
  });
  if (!response.ok) {
    throw new Error(`API request failed with status ${response.status}`);
  }
  const result = await response.json();
  if (result.success && result.data !== undefined) {
    return result.data;
  }
  throw new Error(`Invalid data format from ${endpoint}`);
};

const postToApi = async (endpoint, data, token) => {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || `Failed with status ${response.status}`);
  }
  const result = await response.json();
  return result;
};

const putToApi = async (endpoint, data, token) => {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || `Failed with status ${response.status}`);
  }
  const result = await response.json();
  return result;
};

const deleteFromApi = async (endpoint, token) => {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: 'DELETE',
    headers: { 'Authorization': `Bearer ${token}` },
  });
  if (!response.ok) {
    throw new Error(`Delete failed with status ${response.status}`);
  }
  const result = await response.json();
  return result;
};

// Settings Configuration - defines all manageable entities
const SETTINGS_CONFIG = [
  {
    key: 'gradeLevel',
    title: 'Grade Levels',
    icon: Layers,
    endpoint: '/Grade',
    myEndpoint: '/Grade/my-grade-levels',
    color: 'blue',
    fields: [
      { name: 'name', label: 'Grade Name', type: 'text', required: true, placeholder: 'e.g., Grade 6' },
      { name: 'level', label: 'Level', type: 'number', required: true, placeholder: 'e.g., 6' },
    ]
  },
  {
    key: 'class',
    title: 'Classes',
    icon: Users,
    endpoint: '/Class',
    myEndpoint: '/Class/my-classes',
    color: 'green',
    requiresGrade: true,
    fields: [
      { name: 'name', label: 'Class Name', type: 'text', required: true, placeholder: 'e.g., 6A' },
      { name: 'gradeLevelId', label: 'Grade Level', type: 'select', required: true },
    ]
  },
  {
    key: 'objective',
    title: 'Learning Objectives',
    icon: Target,
    endpoint: '/Objective/template',
    myEndpoint: '/Objective/template/my-templates',
    color: 'red',
    fields: [
      { name: 'name', label: 'Objective Name', type: 'text', required: true, placeholder: 'e.g., Knowledge' },
      { name: 'content', label: 'Content', type: 'textarea', required: true, placeholder: 'Describe the objective...' },
    ]
  },
  {
    key: 'skill',
    title: 'Skills',
    icon: Brain,
    endpoint: '/Skill/template',
    myEndpoint: '/Skill/template/my-templates',
    color: 'purple',
    fields: [
      { name: 'name', label: 'Skill Name', type: 'text', required: true, placeholder: 'e.g., Reading' },
      { name: 'description', label: 'Description', type: 'textarea', required: true, placeholder: 'Describe the skill...' },
    ]
  },
  {
    key: 'attitude',
    title: 'Attitudes',
    icon: Heart,
    endpoint: '/Attitude/template',
    myEndpoint: '/Attitude/template/my-templates',
    color: 'pink',
    fields: [
      { name: 'name', label: 'Attitude Name', type: 'text', required: true, placeholder: 'e.g., Respect' },
      { name: 'content', label: 'Content', type: 'textarea', required: true, placeholder: 'Describe the attitude...' },
    ]
  },
  {
    key: 'preparation',
    title: 'Preparation Types',
    icon: Book,
    endpoint: '/Preparation',
    myEndpoint: '/Preparation/my-types',
    color: 'yellow',
    fields: [
      { name: 'name', label: 'Type Name', type: 'text', required: true, placeholder: 'e.g., Materials' },
      { name: 'description', label: 'Description', type: 'textarea', required: false, placeholder: 'Describe the preparation type...' },
    ]
  },
  {
    key: 'languageFocus',
    title: 'Language Focus Types',
    icon: MessageSquare,
    endpoint: '/Language',
    myEndpoint: '/Language/my-types',
    color: 'indigo',
    fields: [
      { name: 'name', label: 'Type Name', type: 'text', required: true, placeholder: 'e.g., Vocabulary, Grammar' },
      { name: 'description', label: 'Description', type: 'textarea', required: false, placeholder: 'Describe this type...' },
    ]
  },
  {
    key: 'method',
    title: 'Teaching Methods',
    icon: Lightbulb,
    endpoint: '/Method/template',
    myEndpoint: '/Method/template/my-templates',
    color: 'orange',
    fields: [
      { name: 'name', label: 'Method Name', type: 'text', required: true, placeholder: 'e.g., Task-based learning' },
      { name: 'description', label: 'Description', type: 'textarea', required: false, placeholder: 'Describe the method...' },
    ]
  },
  {
    key: 'activity',
    title: 'Activity Templates',
    icon: Activity,
    endpoint: '/Activity/template',
    myEndpoint: '/Activity/template/my-templates',
    color: 'teal',
    fields: [
      { name: 'name', label: 'Activity Name', type: 'text', required: true, placeholder: 'e.g., Group Discussion' },
      { name: 'content', label: 'Content', type: 'textarea', required: true, placeholder: 'Describe the activity...' },
    ]
  },
  {
    key: 'interaction',
    title: 'Interaction Patterns',
    icon: Users,
    endpoint: '/Interaction/pattern',
    myEndpoint: '/Interaction/pattern/my-patterns',
    color: 'cyan',
    fields: [
      { name: 'name', label: 'Pattern Name', type: 'text', required: true, placeholder: 'e.g., Teacher to Students' },
      { name: 'shortCode', label: 'Short Code', type: 'text', required: true, placeholder: 'e.g., T-S' },
      { name: 'description', label: 'Description', type: 'textarea', required: false, placeholder: 'Describe the pattern...' },
    ]
  },
];

export default function LessonSettings() {
  const [activeTab, setActiveTab] = useState('gradeLevel');
  const [data, setData] = useState({});
  const [gradeLevels, setGradeLevels] = useState([]);
  const [skillTypes, setSkillTypes] = useState([]);
  const [loading, setLoading] = useState({});
  const [editingItem, setEditingItem] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({});
  const [message, setMessage] = useState({ type: '', text: '' });
  const [showSkillTypeForm, setShowSkillTypeForm] = useState(false);
  const [newSkillType, setNewSkillType] = useState({ name: '', description: '' });

  const currentConfig = SETTINGS_CONFIG.find(s => s.key === activeTab);

  // Load data for current tab
  useEffect(() => {
    loadData(activeTab);
  }, [activeTab]);

  // Load grade levels for class dropdown
  useEffect(() => {
    const loadGradeLevels = async () => {
      try {
        const token = getAuthToken();
        const grades = await fetchFromApi('/Grade/my-grade-levels', token);
        setGradeLevels(grades);
      } catch (error) {
        console.error('Failed to load grade levels:', error);
      }
    };
    loadGradeLevels();
  }, []);

  // Load skill types for skill dropdown
  useEffect(() => {
    const loadSkillTypes = async () => {
      try {
        const token = getAuthToken();
        const types = await fetchFromApi('/SkillType/my-skill-types', token);
        setSkillTypes(types);
      } catch (error) {
        console.error('Failed to load skill types:', error);
      }
    };
    loadSkillTypes();
  }, []);

  const loadData = async (key) => {
    if (data[key]) return; // Already loaded

    setLoading(prev => ({ ...prev, [key]: true }));
    try {
      const config = SETTINGS_CONFIG.find(s => s.key === key);
      const token = getAuthToken();
      const result = await fetchFromApi(config.myEndpoint, token);
      setData(prev => ({ ...prev, [key]: result }));
      showMessage('success', 'Data loaded successfully');
    } catch (error) {
      showMessage('error', `Failed to load data: ${error.message}`);
    } finally {
      setLoading(prev => ({ ...prev, [key]: false }));
    }
  };

  const showMessage = (type, text) => {
    setMessage({ type, text });
    setTimeout(() => setMessage({ type: '', text: '' }), 5000);
  };

  const handleAdd = () => {
    setEditingItem(null);
    setFormData({});
    setShowForm(true);
  };

  const handleEdit = (item) => {
    setEditingItem(item);
    setFormData(item);
    setShowForm(true);
  };

  const handleDelete = async (id) => {
    if (!confirm('Are you sure you want to delete this item?')) return;

    try {
      const token = getAuthToken();
      await deleteFromApi(`${currentConfig.endpoint}/${id}`, token);

      // Refresh data for current tab
      const result = await fetchFromApi(currentConfig.myEndpoint, token);
      setData(prev => ({ ...prev, [activeTab]: result }));

      // If we just deleted a grade level, refresh grade levels for class dropdown
      if (activeTab === 'gradeLevel') {
        const grades = await fetchFromApi('/Grade/my-grade-levels', token);
        setGradeLevels(grades);
      }

      showMessage('success', 'Item deleted successfully');
    } catch (error) {
      showMessage('error', `Failed to delete: ${error.message}`);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      // Validate input data
      const validationError = validateFormData(formData, currentConfig);
      if (validationError) {
        showMessage('error', validationError);
        return;
      }

      const token = getAuthToken();

      if (editingItem) {
        // Update existing
        await putToApi(`${currentConfig.endpoint}/${editingItem.id}`, formData, token);
        showMessage('success', 'Item updated successfully');
      } else {
        // Create new
        await postToApi(currentConfig.endpoint, formData, token);
        showMessage('success', 'Item created successfully');
      }

      // Refresh data for current tab
      const result = await fetchFromApi(currentConfig.myEndpoint, token);
      setData(prev => ({ ...prev, [activeTab]: result }));

      // If we just created/updated a grade level, refresh grade levels for class dropdown
      if (activeTab === 'gradeLevel') {
        const grades = await fetchFromApi('/Grade/my-grade-levels', token);
        setGradeLevels(grades);
      }

      setShowForm(false);
      setFormData({});
      setEditingItem(null);
    } catch (error) {
      showMessage('error', `Failed to save: ${error.message}`);
    }
  };

  // Validation function
  const validateFormData = (data, config) => {
    // Check required fields
    for (const field of config.fields) {
      if (field.required && !data[field.name]) {
        return `${field.label} is required`;
      }

      const value = data[field.name];

      // Validate text fields
      if (field.type === 'text' && value) {
        const trimmedValue = value.trim();
        if (trimmedValue.length === 0) {
          return `${field.label} cannot be empty or whitespace only`;
        }
        if (trimmedValue.length > 200) {
          return `${field.label} must be 200 characters or less`;
        }
      }

      // Validate textarea fields
      if (field.type === 'textarea' && value) {
        const trimmedValue = value.trim();
        if (field.required && trimmedValue.length === 0) {
          return `${field.label} cannot be empty or whitespace only`;
        }
        if (trimmedValue.length > 2000) {
          return `${field.label} must be 2000 characters or less`;
        }
      }

      // Validate number fields (e.g., grade level)
      if (field.type === 'number' && value !== undefined && value !== null && value !== '') {
        const numValue = Number(value);
        if (isNaN(numValue)) {
          return `${field.label} must be a valid number`;
        }
        if (field.name === 'level') {
          if (numValue < 1 || numValue > 12) {
            return `${field.label} must be between 1 and 12`;
          }
          if (!Number.isInteger(numValue)) {
            return `${field.label} must be a whole number`;
          }
        }
        if (numValue < 0) {
          return `${field.label} cannot be negative`;
        }
      }

      // Validate short code (for interaction patterns)
      if (field.name === 'shortCode' && value) {
        const trimmedValue = value.trim();
        if (trimmedValue.length > 10) {
          return `${field.label} must be 10 characters or less`;
        }
      }
    }

    return null; // No validation errors
  };

  const handleInputChange = (field, value) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleCreateSkillType = async () => {
    const trimmedName = newSkillType.name.trim();
    
    if (!trimmedName) {
      showMessage('error', 'Skill type name is required');
      return;
    }

    if (trimmedName.length > 200) {
      showMessage('error', 'Skill type name must be 200 characters or less');
      return;
    }

    if (newSkillType.description && newSkillType.description.trim().length > 2000) {
      showMessage('error', 'Description must be 2000 characters or less');
      return;
    }

    try {
      const token = getAuthToken();
      const skillTypeData = {
        name: trimmedName,
        description: newSkillType.description.trim() || undefined
      };
      const result = await postToApi('/SkillType', skillTypeData, token);

      // Refresh skill types
      const types = await fetchFromApi('/SkillType/my-skill-types', token);
      setSkillTypes(types);

      // Set the newly created skill type as selected
      setFormData(prev => ({ ...prev, skillTypeId: result.data.id }));

      // Close the skill type form and clear inputs
      setShowSkillTypeForm(false);
      setNewSkillType({ name: '', description: '' });

      showMessage('success', 'Skill type created successfully');
    } catch (error) {
      showMessage('error', `Failed to create skill type: ${error.message}`);
    }
  };

  const renderField = (field) => {
    const value = formData[field.name] || '';

    if (field.type === 'select' && field.name === 'gradeLevelId') {
      return (
        <select
          value={value}
          onChange={(e) => handleInputChange(field.name, parseInt(e.target.value))}
          className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          required={field.required}
        >
          <option value="">Select Grade Level</option>
          {gradeLevels.map(grade => (
            <option key={grade.id} value={grade.id}>{grade.name}</option>
          ))}
        </select>
      );
    }

    if (field.type === 'select-skilltype') {
      return (
        <div className="space-y-2">
          <div className="flex gap-2">
            <select
              value={value}
              onChange={(e) => handleInputChange(field.name, parseInt(e.target.value))}
              className="flex-1 p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
              required={field.required}
            >
              <option value="">Select Skill Type</option>
              {skillTypes.map(type => (
                <option key={type.id} value={type.id}>{type.name}</option>
              ))}
            </select>
            <button
              type="button"
              onClick={() => setShowSkillTypeForm(!showSkillTypeForm)}
              className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition flex items-center gap-2"
              title="Create new skill type"
            >
              <Plus className="w-4 h-4" />
              <span>New</span>
            </button>
          </div>

          {showSkillTypeForm && (
            <div className="p-4 border border-purple-200 bg-purple-50 rounded-lg space-y-3">
              <label className="block text-sm font-semibold text-gray-700">
                Create New Skill Type
              </label>
              <div className="space-y-3">
                <input
                  type="text"
                  value={newSkillType.name}
                  onChange={(e) => setNewSkillType({ ...newSkillType, name: e.target.value })}
                  placeholder="e.g., Language, Social, Motor..."
                  className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                  maxLength={200}
                  onKeyPress={(e) => {
                    if (e.key === 'Enter' && !e.shiftKey) {
                      e.preventDefault();
                      handleCreateSkillType();
                    }
                  }}
                />
                <textarea
                  value={newSkillType.description}
                  onChange={(e) => setNewSkillType({ ...newSkillType, description: e.target.value })}
                  placeholder="Description (optional)..."
                  rows="2"
                  className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                  maxLength={2000}
                />
                <div className="flex gap-2">
                  <button
                    type="button"
                    onClick={handleCreateSkillType}
                    className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition flex items-center gap-2"
                  >
                    <Save className="w-4 h-4" />
                    <span>Save</span>
                  </button>
                  <button
                    type="button"
                    onClick={() => {
                      setShowSkillTypeForm(false);
                      setNewSkillType({ name: '', description: '' });
                    }}
                    className="px-4 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition"
                  >
                    <X className="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          )}
        </div>
      );
    }

    if (field.type === 'textarea') {
      return (
        <textarea
          value={value}
          onChange={(e) => handleInputChange(field.name, e.target.value)}
          placeholder={field.placeholder}
          rows="4"
          className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          required={field.required}
          maxLength={2000}
        />
      );
    }

    return (
      <input
        type={field.type}
        value={value}
        onChange={(e) => handleInputChange(field.name, field.type === 'number' ? parseInt(e.target.value) || '' : e.target.value)}
        placeholder={field.placeholder}
        className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
        required={field.required}
        {...(field.type === 'text' && { maxLength: 200 })}
        {...(field.type === 'number' && field.name === 'level' && { min: 1, max: 12, step: 1 })}
        {...(field.type === 'number' && field.name !== 'level' && { min: 0 })}
        {...(field.name === 'shortCode' && { maxLength: 10 })}
      />
    );
  };

  const currentData = data[activeTab] || [];
  const isLoading = loading[activeTab];

  return (
    <div className="min-h-screen bg-gray-50 p-4 sm:p-8">
      <header className="mb-8">
        <h1 className="text-3xl font-extrabold text-gray-900 flex items-center">
          <Settings className="w-8 h-8 mr-3 text-blue-600" />
          Lesson Planner Settings
        </h1>
        <p className="text-gray-500 mt-1">Manage all your lesson planning templates and configurations</p>
      </header>

      {/* Message Banner */}
      {message.text && (
        <div className={`mb-4 p-4 rounded-lg flex items-center space-x-2 animate-fade-in ${message.type === 'success' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
          }`}>
          {message.type === 'success' ? (
            <CheckCircle2 className="w-5 h-5" />
          ) : (
            <AlertCircle className="w-5 h-5" />
          )}
          <span>{message.text}</span>
        </div>
      )}

      <div className="flex flex-col lg:flex-row gap-6">
        {/* Sidebar Navigation */}
        <div className="lg:w-64 bg-white rounded-xl shadow-lg p-4 h-fit sticky top-4">
          <h3 className="text-sm font-bold text-gray-500 uppercase mb-3">Categories</h3>
          <nav className="space-y-1">
            {SETTINGS_CONFIG.map((config) => {
              const Icon = config.icon;
              const isActive = activeTab === config.key;
              return (
                <button
                  key={config.key}
                  onClick={() => setActiveTab(config.key)}
                  className={`w-full flex items-center space-x-3 px-4 py-3 rounded-lg transition-all ${isActive
                    ? `bg-${config.color}-100 text-${config.color}-700 font-semibold`
                    : 'text-gray-700 hover:bg-gray-100'
                    }`}
                >
                  <Icon className="w-5 h-5" />
                  <span className="text-sm">{config.title}</span>
                </button>
              );
            })}
          </nav>
        </div>

        {/* Main Content */}
        <div className="flex-1">
          <div className="bg-white rounded-xl shadow-lg p-6">
            {/* Header */}
            <div className="flex justify-between items-center mb-6 pb-4 border-b">
              <div className="flex items-center space-x-3">
                {currentConfig && React.createElement(currentConfig.icon, {
                  className: `w-6 h-6 text-${currentConfig.color}-600`
                })}
                <h2 className="text-2xl font-bold text-gray-900">{currentConfig?.title}</h2>
              </div>
              <button
                onClick={handleAdd}
                className="flex items-center space-x-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition shadow"
              >
                <Plus className="w-5 h-5" />
                <span>Add New</span>
              </button>
            </div>

            {/* Form Modal */}
            {showForm && (
              <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                <div className="bg-white rounded-xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
                  <div className="p-6 border-b flex justify-between items-center sticky top-0 bg-white">
                    <h3 className="text-xl font-bold">
                      {editingItem ? 'Edit' : 'Add New'} {currentConfig?.title}
                    </h3>
                    <button
                      onClick={() => {
                        setShowForm(false);
                        setFormData({});
                        setEditingItem(null);
                      }}
                      className="text-gray-500 hover:text-gray-700"
                    >
                      <X className="w-6 h-6" />
                    </button>
                  </div>

                  <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    {currentConfig?.fields.map((field) => (
                      <div key={field.name}>
                        <label className="block text-sm font-bold text-gray-700 mb-2">
                          {field.label}
                          {field.required && <span className="text-red-500 ml-1">*</span>}
                        </label>
                        {renderField(field)}
                      </div>
                    ))}

                    <div className="flex justify-end space-x-3 pt-4">
                      <button
                        type="button"
                        onClick={() => {
                          setShowForm(false);
                          setFormData({});
                          setEditingItem(null);
                        }}
                        className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300"
                      >
                        Cancel
                      </button>
                      <button
                        type="submit"
                        className="flex items-center space-x-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                      >
                        <Save className="w-4 h-4" />
                        <span>Save</span>
                      </button>
                    </div>
                  </form>
                </div>
              </div>
            )}

            {/* Data List */}
            {isLoading ? (
              <div className="flex items-center justify-center py-12">
                <Loader2 className="w-8 h-8 text-blue-600 animate-spin" />
              </div>
            ) : currentData.length === 0 ? (
              <div className="text-center py-12 text-gray-500">
                <BookOpen className="w-16 h-16 mx-auto mb-4 opacity-50" />
                <p className="text-lg">No {currentConfig?.title.toLowerCase()} found</p>
                <p className="text-sm mt-2">Click "Add New" to create your first entry</p>
              </div>
            ) : (
              <div className="space-y-3">
                {currentData.map((item) => (
                  <div
                    key={item.id}
                    className="p-4 border border-gray-200 rounded-lg hover:shadow-md transition-shadow"
                  >
                    <div className="flex justify-between items-start">
                      <div className="flex-1 min-w-0">
                        <h4 className="font-bold text-gray-900 truncate break-all">
                          {item.name}
                        </h4>
                        {item.level && (
                          <p className="text-sm text-gray-600">Level: {item.level}</p>
                        )}
                        {item.gradeLevelName && (
                          <p className="text-sm text-gray-600 truncate break-all">Grade: {item.gradeLevelName}</p>
                        )}
                        {item.skillTypeName && (
                          <p className="text-sm text-gray-600 truncate break-all">Type: {item.skillTypeName}</p>
                        )}
                        {item.shortCode && (
                          <p className="text-sm text-gray-600 truncate break-all">Code: {item.shortCode}</p>
                        )}
                        {(item.content || item.description) && (
                          <p className="text-sm text-gray-600 mt-2 line-clamp-2 break-all">
                            {item.content || item.description}
                          </p>
                        )}
                      </div>
                      <div className="flex space-x-2 ml-4">
                        <button
                          onClick={() => handleEdit(item)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition"
                          title="Edit"
                        >
                          <Edit2 className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDelete(item.id)}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition"
                          title="Delete"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
