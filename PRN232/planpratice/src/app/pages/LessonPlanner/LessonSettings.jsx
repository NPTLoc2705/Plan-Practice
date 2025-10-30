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

// =================================================================
// LESSON SETTINGS MANAGEMENT PAGE
// =================================================================
// This page provides a unified interface to manage all lesson planner settings:
// - Grade Levels (GET/POST/PUT/DELETE: /api/GradeLevel)
// - Classes (GET/POST/PUT/DELETE: /api/Class)
// - Objective Templates (GET/POST/PUT/DELETE: /api/ObjectiveTemplate)
// - Skill Templates (GET/POST/PUT/DELETE: /api/SkillTemplate)
// - Attitude Templates (GET/POST/PUT/DELETE: /api/AttitudeTemplate)
// - Preparation Types (GET/POST/PUT/DELETE: /api/PreparationType)
// - Language Focus Types (GET/POST/PUT/DELETE: /api/LanguageFocusType)
// - Method Templates (GET/POST/PUT/DELETE: /api/MethodTemplate)
// - Activity Templates (GET/POST/PUT/DELETE: /api/ActivityTemplate)
// - Interaction Patterns (GET/POST/PUT/DELETE: /api/InteractionPattern)
// =================================================================

const API_BASE_URL = 'https://localhost:7025/api';

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
    endpoint: '/GradeLevel',
    myEndpoint: '/GradeLevel/my-grade-levels',
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
    endpoint: '/ObjectiveTemplate',
    myEndpoint: '/ObjectiveTemplate/my-templates',
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
    endpoint: '/SkillTemplate',
    myEndpoint: '/SkillTemplate/my-templates',
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
    endpoint: '/AttitudeTemplate',
    myEndpoint: '/AttitudeTemplate/my-templates',
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
    endpoint: '/PreparationType',
    myEndpoint: '/PreparationType/my-types',
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
    endpoint: '/LanguageFocusType',
    myEndpoint: '/LanguageFocusType/my-types',
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
    endpoint: '/MethodTemplate',
    myEndpoint: '/MethodTemplate/my-templates',
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
    endpoint: '/ActivityTemplate',
    myEndpoint: '/ActivityTemplate/my-templates',
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
    endpoint: '/InteractionPattern',
    myEndpoint: '/InteractionPattern/my-patterns',
    color: 'cyan',
    fields: [
      { name: 'name', label: 'Pattern Name', type: 'text', required: true, placeholder: 'e.g., Teacher to Students' },
      { name: 'shortCode', label: 'Short Code', type: 'text', required: true, placeholder: 'e.g., T-S' },
      { name: 'description', label: 'Description', type: 'textarea', required: false, placeholder: 'Describe the pattern...' },
    ]
  },
  {
    key: 'activityStage',
    title: 'Activity Stage Templates',
    icon: Layers,
    endpoint: null, // Local storage for now
    myEndpoint: null,
    color: 'emerald',
    isLocal: true, // Flag for localStorage-based management
    fields: []
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
  
  // Activity Stage Template specific states
  const [activityTemplates, setActivityTemplates] = useState([]);
  const [interactionPatterns, setInteractionPatterns] = useState([]);
  const [showStageForm, setShowStageForm] = useState(false);
  const [editingStage, setEditingStage] = useState(null);
  const [stageFormData, setStageFormData] = useState({
    name: '',
    subActivities: [
      { timeInMinutes: 5, activityTemplateId: '', customContent: '', interactionPatternId: '' }
    ]
  });

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
        const grades = await fetchFromApi('/GradeLevel/my-grade-levels', token);
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

  // Load activity templates and interaction patterns for activity stage builder
  useEffect(() => {
    const loadActivityData = async () => {
      try {
        const token = getAuthToken();
        const [templates, patterns] = await Promise.all([
          fetchFromApi('/ActivityTemplate/my-templates', token),
          fetchFromApi('/InteractionPattern/my-patterns', token)
        ]);
        setActivityTemplates(templates);
        setInteractionPatterns(patterns);
      } catch (error) {
        console.error('Failed to load activity data:', error);
      }
    };
    loadActivityData();
  }, []);

  const loadData = async (key) => {
    if (data[key]) return; // Already loaded

    setLoading(prev => ({ ...prev, [key]: true }));
    try {
      const config = SETTINGS_CONFIG.find(s => s.key === key);
      
      // Handle localStorage-based data (activity stages)
      if (config.isLocal) {
        const localData = localStorage.getItem('activityStageTemplates');
        const stages = localData ? JSON.parse(localData) : [];
        setData(prev => ({ ...prev, [key]: stages }));
        showMessage('success', 'Data loaded successfully');
      } else {
        const token = getAuthToken();
        const result = await fetchFromApi(config.myEndpoint, token);
        setData(prev => ({ ...prev, [key]: result }));
        showMessage('success', 'Data loaded successfully');
      }
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
    if (activeTab === 'activityStage') {
      handleAddStage();
    } else {
      setEditingItem(null);
      setFormData({});
      setShowForm(true);
    }
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

      // Refresh data
      const result = await fetchFromApi(currentConfig.myEndpoint, token);
      setData(prev => ({ ...prev, [activeTab]: result }));

      showMessage('success', 'Item deleted successfully');
    } catch (error) {
      showMessage('error', `Failed to delete: ${error.message}`);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
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

      // Refresh data
      const result = await fetchFromApi(currentConfig.myEndpoint, token);
      setData(prev => ({ ...prev, [activeTab]: result }));

      setShowForm(false);
      setFormData({});
      setEditingItem(null);
    } catch (error) {
      showMessage('error', `Failed to save: ${error.message}`);
    }
  };

  const handleInputChange = (field, value) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  // Activity Stage Template handlers
  const handleAddStage = () => {
    setEditingStage(null);
    setStageFormData({
      name: '',
      subActivities: [
        { timeInMinutes: 5, activityTemplateId: '', customContent: '', interactionPatternId: '' }
      ]
    });
    setShowStageForm(true);
  };

  const handleEditStage = (stage) => {
    setEditingStage(stage);
    setStageFormData(stage);
    setShowStageForm(true);
  };

  const handleDeleteStage = (id) => {
    if (!confirm('Are you sure you want to delete this activity stage?')) return;

    const stages = data.activityStage || [];
    const updated = stages.filter(s => s.id !== id);
    localStorage.setItem('activityStageTemplates', JSON.stringify(updated));
    setData(prev => ({ ...prev, activityStage: updated }));
    showMessage('success', 'Activity stage deleted successfully');
  };

  const handleSaveStage = (e) => {
    e.preventDefault();

    if (!stageFormData.name.trim()) {
      showMessage('error', 'Please enter a stage name');
      return;
    }

    const stages = data.activityStage || [];
    let updated;

    if (editingStage) {
      // Update existing
      updated = stages.map(s => s.id === editingStage.id ? { ...stageFormData, id: editingStage.id } : s);
    } else {
      // Create new
      const newStage = {
        ...stageFormData,
        id: Date.now() // Simple ID generation
      };
      updated = [...stages, newStage];
    }

    localStorage.setItem('activityStageTemplates', JSON.stringify(updated));
    setData(prev => ({ ...prev, activityStage: updated }));
    setShowStageForm(false);
    setStageFormData({
      name: '',
      subActivities: [
        { timeInMinutes: 5, activityTemplateId: '', customContent: '', interactionPatternId: '' }
      ]
    });
    showMessage('success', editingStage ? 'Activity stage updated successfully' : 'Activity stage created successfully');
  };

  const addSubActivityToStage = () => {
    setStageFormData(prev => ({
      ...prev,
      subActivities: [
        ...prev.subActivities,
        { timeInMinutes: 5, activityTemplateId: '', customContent: '', interactionPatternId: '' }
      ]
    }));
  };

  const removeSubActivityFromStage = (index) => {
    setStageFormData(prev => ({
      ...prev,
      subActivities: prev.subActivities.filter((_, i) => i !== index)
    }));
  };

  const updateStageSubActivity = (index, field, value) => {
    setStageFormData(prev => ({
      ...prev,
      subActivities: prev.subActivities.map((sub, i) =>
        i === index ? { ...sub, [field]: value } : sub
      )
    }));
  };

  const handleCreateSkillType = async () => {
    if (!newSkillType.name.trim()) {
      showMessage('error', 'Please enter a skill type name');
      return;
    }

    try {
      const token = getAuthToken();
      const skillTypeData = {
        name: newSkillType.name,
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
        />
      );
    }

    return (
      <input
        type={field.type}
        value={value}
        onChange={(e) => handleInputChange(field.name, field.type === 'number' ? parseInt(e.target.value) : e.target.value)}
        placeholder={field.placeholder}
        className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
        required={field.required}
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
            ) : activeTab === 'activityStage' ? (
              // Activity Stage Template specific UI
              <>
                {showStageForm && (
                  <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                    <div className="bg-white rounded-xl shadow-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
                      <div className="p-6 border-b flex justify-between items-center sticky top-0 bg-white z-10">
                        <h3 className="text-xl font-bold">
                          {editingStage ? 'Edit' : 'Add New'} Activity Stage Template
                        </h3>
                        <button
                          onClick={() => {
                            setShowStageForm(false);
                            setStageFormData({
                              name: '',
                              subActivities: [
                                { timeInMinutes: 5, activityTemplateId: '', customContent: '', interactionPatternId: '' }
                              ]
                            });
                          }}
                          className="text-gray-500 hover:text-gray-700"
                        >
                          <X className="w-6 h-6" />
                        </button>
                      </div>

                      <form onSubmit={handleSaveStage} className="p-6 space-y-6">
                        <div>
                          <label className="block text-sm font-bold text-gray-700 mb-2">
                            Stage Name <span className="text-red-500">*</span>
                          </label>
                          <input
                            type="text"
                            value={stageFormData.name}
                            onChange={(e) => setStageFormData(prev => ({ ...prev, name: e.target.value }))}
                            placeholder="e.g., Warm up, New lesson, Practice..."
                            className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            required
                          />
                        </div>

                        <div className="space-y-4">
                          <div className="flex justify-between items-center">
                            <h4 className="font-bold text-gray-700">Activities</h4>
                            <button
                              type="button"
                              onClick={addSubActivityToStage}
                              className="flex items-center gap-2 px-3 py-1.5 bg-blue-100 text-blue-800 rounded-lg hover:bg-blue-200"
                            >
                              <Plus className="w-4 h-4" />
                              <span>Add Activity</span>
                            </button>
                          </div>

                          {stageFormData.subActivities.map((sub, index) => (
                            <div key={index} className="p-4 border-2 border-gray-200 rounded-lg bg-gray-50 space-y-3 relative">
                              <span className="absolute -left-2 top-3 text-xs bg-blue-500 text-white font-bold rounded-full h-6 w-6 flex items-center justify-center">
                                {index + 1}
                              </span>
                              
                              {stageFormData.subActivities.length > 1 && (
                                <button
                                  type="button"
                                  onClick={() => removeSubActivityFromStage(index)}
                                  className="absolute top-2 right-2 text-red-500 hover:text-red-700"
                                >
                                  <X className="w-4 h-4" />
                                </button>
                              )}

                              <div className="pl-5 space-y-3">
                                <div>
                                  <label className="block text-xs font-semibold text-gray-600 mb-1">Time (minutes)</label>
                                  <input
                                    type="number"
                                    min="0"
                                    value={sub.timeInMinutes}
                                    onChange={(e) => updateStageSubActivity(index, 'timeInMinutes', parseInt(e.target.value) || 0)}
                                    className="w-full p-2 border border-gray-300 rounded text-sm"
                                  />
                                </div>

                                <div>
                                  <label className="block text-xs font-semibold text-gray-600 mb-1">Activity Template</label>
                                  <select
                                    value={sub.activityTemplateId}
                                    onChange={(e) => updateStageSubActivity(index, 'activityTemplateId', e.target.value)}
                                    className="w-full p-2 border border-gray-300 rounded text-sm"
                                  >
                                    <option value="">-- Select a template or use custom --</option>
                                    {activityTemplates.map(template => (
                                      <option key={template.id} value={template.id}>{template.name}</option>
                                    ))}
                                  </select>
                                  {sub.activityTemplateId && activityTemplates.find(t => t.id === parseInt(sub.activityTemplateId)) && (
                                    <div className="mt-2 p-2 bg-blue-50 border border-blue-200 rounded text-xs">
                                      <strong>Template Content:</strong>
                                      <p className="mt-1 whitespace-pre-wrap">
                                        {activityTemplates.find(t => t.id === parseInt(sub.activityTemplateId))?.content}
                                      </p>
                                    </div>
                                  )}
                                </div>

                                {!sub.activityTemplateId && (
                                  <div>
                                    <label className="block text-xs font-semibold text-gray-600 mb-1">Custom Activity Content</label>
                                    <textarea
                                      rows="3"
                                      placeholder="Describe the activities..."
                                      value={sub.customContent}
                                      onChange={(e) => updateStageSubActivity(index, 'customContent', e.target.value)}
                                      className="w-full p-2 border border-gray-300 rounded text-sm"
                                    />
                                  </div>
                                )}

                                <div>
                                  <label className="block text-xs font-semibold text-gray-600 mb-1">Interaction Pattern</label>
                                  <select
                                    value={sub.interactionPatternId}
                                    onChange={(e) => updateStageSubActivity(index, 'interactionPatternId', e.target.value)}
                                    className="w-full p-2 border border-gray-300 rounded text-sm"
                                  >
                                    <option value="">Select interaction</option>
                                    {interactionPatterns.map(pattern => (
                                      <option key={pattern.id} value={pattern.id}>
                                        {pattern.name} ({pattern.shortCode})
                                      </option>
                                    ))}
                                  </select>
                                </div>
                              </div>
                            </div>
                          ))}
                        </div>

                        <div className="flex justify-end space-x-3 pt-4 border-t">
                          <button
                            type="button"
                            onClick={() => {
                              setShowStageForm(false);
                              setStageFormData({
                                name: '',
                                subActivities: [
                                  { timeInMinutes: 5, activityTemplateId: '', customContent: '', interactionPatternId: '' }
                                ]
                              });
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
                            <span>Save Stage Template</span>
                          </button>
                        </div>
                      </form>
                    </div>
                  </div>
                )}

                {currentData.length === 0 ? (
                  <div className="text-center py-12 text-gray-500">
                    <Layers className="w-16 h-16 mx-auto mb-4 opacity-50" />
                    <p className="text-lg">No activity stage templates found</p>
                    <p className="text-sm mt-2">Create reusable activity stages to speed up lesson planning</p>
                  </div>
                ) : (
                  <div className="space-y-3">
                    {currentData.map((stage) => (
                      <div
                        key={stage.id}
                        className="p-4 border border-gray-200 rounded-lg hover:shadow-md transition-shadow"
                      >
                        <div className="flex justify-between items-start mb-3">
                          <div className="flex-1 min-w-0">
                            <h4 className="font-bold text-gray-900 text-lg truncate break-all">{stage.name}</h4>
                            <p className="text-sm text-gray-600 mt-1">
                              {stage.subActivities.length} {stage.subActivities.length === 1 ? 'activity' : 'activities'} • 
                              Total: {stage.subActivities.reduce((sum, sub) => sum + (sub.timeInMinutes || 0), 0)} min
                            </p>
                          </div>
                          <div className="flex space-x-2 ml-4">
                            <button
                              onClick={() => handleEditStage(stage)}
                              className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition"
                              title="Edit"
                            >
                              <Edit2 className="w-4 h-4" />
                            </button>
                            <button
                              onClick={() => handleDeleteStage(stage.id)}
                              className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition"
                              title="Delete"
                            >
                              <Trash2 className="w-4 h-4" />
                            </button>
                          </div>
                        </div>

                        <div className="space-y-2">
                          {stage.subActivities.map((sub, idx) => {
                            const template = activityTemplates.find(t => t.id === parseInt(sub.activityTemplateId));
                            const pattern = interactionPatterns.find(p => p.id === parseInt(sub.interactionPatternId));
                            const content = template ? template.content : sub.customContent;
                            
                            return (
                              <div key={idx} className="flex items-start gap-3 p-3 bg-gray-50 rounded border border-gray-200">
                                <div className="flex-shrink-0 w-8 h-8 bg-blue-500 text-white rounded-full flex items-center justify-center text-xs font-bold">
                                  {idx + 1}
                                </div>
                                <div className="flex-1 min-w-0">
                                  <div className="flex items-center gap-2 mb-1">
                                    <span className="text-xs font-semibold text-gray-600">
                                      {sub.timeInMinutes} min
                                    </span>
                                    {template && (
                                      <span className="text-xs bg-purple-100 text-purple-700 px-2 py-0.5 rounded">
                                        {template.name}
                                      </span>
                                    )}
                                    {pattern && (
                                      <span className="text-xs bg-cyan-100 text-cyan-700 px-2 py-0.5 rounded">
                                        {pattern.shortCode}
                                      </span>
                                    )}
                                  </div>
                                  <p className="text-sm text-gray-700 line-clamp-2 break-all">
                                    {content || 'No content'}
                                  </p>
                                </div>
                              </div>
                            );
                          })}
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </>
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
