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
  ImageIcon,
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
  ChevronRight,
  ChevronLeft,
  Plus,
  X,
  Calendar,
  Clock,
  Target,
  Brain,
  Heart,
  Book,
  Users,
  Settings,
  CheckCircle2,
} from 'lucide-react';

// =================================================================
// MOCK BACKEND DATA (InteractionPatterns removed)
// =================================================================
const mockDatabase = {

  classes: [
    { id: 101, gradeId: 1, name: 'Class 10A1' },
    { id: 102, gradeId: 1, name: 'Class 10A2' },
    { id: 103, gradeId: 1, name: 'Class 10B' },
    { id: 201, gradeId: 2, name: 'Class 11A1' },
    { id: 202, gradeId: 2, name: 'Class 11C' },
    { id: 301, gradeId: 3, name: 'Class 12D' },
  ],

  skillTemplates: [
    { id: 1, skillType: 'Reading', name: 'Reading for general ideas', description: 'Reading for general ideas and specific information' },
    { id: 2, skillType: 'Speaking', name: 'Talking about topics', description: 'Talking about how to protect endangered species' },
    { id: 3, skillType: 'Listening', name: 'Listening comprehension', description: 'Listening for specific information' },
    { id: 4, skillType: 'Writing', name: 'Report writing', description: 'Writing a report about endangered species' },
  ],
  methodTemplates: [
    { id: 1, name: 'Integrated, mainly communicative', description: 'The whole lesson: Integrated, mainly communicative' },
    { id: 2, name: 'Task-based learning', description: 'Task-based approach with pair and group work' },
    { id: 3, name: 'CLT', description: 'Communicative Language Teaching approach' },
  ],
  // InteractionPatterns are now fetched from the API
};

// =================================================================
// MOCK API FETCH FUNCTIONS
// =================================================================
const mockApiFetch = (data, delay = 500) => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve({ success: true, data: data });
    }, delay);
  });
};
const mockFetchGrades = () => mockApiFetch(mockDatabase.gradeLevels);
const mockFetchClassesByGrade = (gradeId) => {
  const filteredClasses = mockDatabase.classes.filter(c => c.gradeId === parseInt(gradeId));
  return mockApiFetch(filteredClasses);
};
const mockSaveLesson = (lessonData) => {
    console.log("Saving lesson data to mock backend:", lessonData);
    const newLesson = {
        ...lessonData,
        id: Date.now(),
        createdAt: new Date().toISOString(),
    };
    return mockApiFetch(newLesson, 1000);
}

export default function App() {
  // ... (all other state declarations remain the same)
  const [currentStep, setCurrentStep] = useState(1);
  const totalSteps = 6;
  const [gradeLevels, setGradeLevels] = useState([]);
  const [classes, setClasses] = useState([]);
  const [selectedGradeId, setSelectedGradeId] = useState('');
  const [selectedClassId, setSelectedClassId] = useState('');
  const [isLoadingGrades, setIsLoadingGrades] = useState(false);
  const [isLoadingClasses, setIsLoadingClasses] = useState(false);
  const [lessonTitle, setLessonTitle] = useState('Lesson on Endangered Species');
  const [lessonDescription, setLessonDescription] = useState('An introductory lesson for Unit 6.');
  const [dateOfPreparation, setDateOfPreparation] = useState(new Date().toISOString().split('T')[0]);
  const [dateOfTeaching, setDateOfTeaching] = useState('');
  const [unitNumber, setUnitNumber] = useState('Unit 6');
  const [unitName, setUnitName] = useState('Endangered Species');
  const [lessonNumber, setLessonNumber] = useState('Lesson 1 - Getting Started');
  const [objectiveTemplates, setObjectiveTemplates] = useState([]);
  const [selectedObjectives, setSelectedObjectives] = useState([]);
  const [objectiveToAdd, setObjectiveToAdd] = useState('');
  const [skillTemplates, setSkillTemplates] = useState([]);
  const [selectedSkills, setSelectedSkills] = useState([]);
  const [skillToAdd, setSkillToAdd] = useState('');
  const [attitudeTemplates, setAttitudeTemplates] = useState([]);
  const [selectedAttitudes, setSelectedAttitudes] = useState([]);
  const [attitudeToAdd, setAttitudeToAdd] = useState('');
  const [languageFocus, setLanguageFocus] = useState([
    { type: 'Vocabulary', content: 'species, extinct, habitat, conservation' },
    { type: 'Grammar', content: 'Conditional sentences type 1' },
    { type: 'Pronunciation', content: '/s/ vs /z/ sounds' },
  ]);
const [preparationTemplates, setPreparationTemplates] = useState([]);
const [selectedPreparations, setSelectedPreparations] = useState([]);
const [preparationToAdd, setPreparationToAdd] = useState('');
  const [methodTemplates, setMethodTemplates] = useState([]);
  const [selectedMethod, setSelectedMethod] = useState('');
  const [interactionPatterns, setInteractionPatterns] = useState([]); // Initial state is empty array
  const [activities, setActivities] = useState([
    {
      stageName: 'Warm up',
      subActivities: [
        { timeInMinutes: 5, content: '- Look at the pictures and name the animals\n- T shows the pictures and has Ss name the animals.\n- T asks Ss the questions to elicit the new lesson.', interactionPatternId: '1' }
      ]
    },
    {
      stageName: 'New lesson',
      subActivities: [
        { timeInMinutes: 17, content: 'Activity 1: Listen and read\n- T. plays the recording, asks Ss to listen and read silently.\n- Ss listen and read silently.', interactionPatternId: '1' },
        { timeInMinutes: 10, content: 'Activity 2: Decide whether the statements are True, False or Not Given.\n- Ask Ss to read the statements individually first.\n- Encourage Ss to provide reasons for their answers.', interactionPatternId: '3' },
      ]
    }
  ]);
  const [message, setMessage] = useState('');
  const contentRef = useRef(null);

  // ... (all handler functions like addStage, removeStage, etc. remain the same)

  const handleAddPreparation = (templateId) => {
    if (!templateId || selectedPreparations.some(p => p.id === parseInt(templateId))) return;
    const template = preparationTemplates.find(t => t.id === parseInt(templateId));
    if (template) {
      setSelectedPreparations([
        ...selectedPreparations,
        // Add the selected template with its default description as the materials
        { id: template.id, name: template.name, materials: template.description || '' }
      ]);
    }
    setPreparationToAdd('');
  };

  const handleRemovePreparation = (templateId) => {
    setSelectedPreparations(selectedPreparations.filter(p => p.id !== templateId));
  };



  const addStage = () => {
    setActivities([...activities, { stageName: 'New Stage', subActivities: [{ timeInMinutes: 5, content: '', interactionPatternId: '' }] }]);
  };
  const removeStage = (stageIndex) => {
    setActivities(activities.filter((_, i) => i !== stageIndex));
  };
  const updateStageName = (stageIndex, newName) => {
    const updatedActivities = [...activities];
    updatedActivities[stageIndex].stageName = newName;
    setActivities(updatedActivities);
  };
  const addSubActivity = (stageIndex) => {
    const updatedActivities = [...activities];
    updatedActivities[stageIndex].subActivities.push({ timeInMinutes: 5, content: '', interactionPatternId: '' });
    setActivities(updatedActivities);
  };
  const removeSubActivity = (stageIndex, subIndex) => {
    const updatedActivities = [...activities];
    updatedActivities[stageIndex].subActivities = updatedActivities[stageIndex].subActivities.filter((_, i) => i !== subIndex);
    if (updatedActivities[stageIndex].subActivities.length === 0) {
        removeStage(stageIndex);
    } else {
        setActivities(updatedActivities);
    }
  };
  const updateSubActivity = (stageIndex, subIndex, field, value) => {
    const updatedActivities = [...activities];
    updatedActivities[stageIndex].subActivities[subIndex][field] = value;
    setActivities(updatedActivities);
  };


  // =================================================================
  // MODIFIED useEffect TO FETCH REAL DATA FOR InteractionPatterns
  // =================================================================
useEffect(() => {
    // --- GENERIC API FETCHER FUNCTION ---
    const fetchFromApi = async (endpoint, token) => {
      const response = await fetch(endpoint, {
        headers: { 'Authorization': `Bearer ${token}` },
      });
      if (!response.ok) {
        throw new Error(`API request to ${endpoint} failed with status ${response.status}`);
      }
      const result = await response.json();
      if (result.success && Array.isArray(result.data)) {
        return result.data;
      }
      throw new Error(`Invalid data format from ${endpoint}`);
    };

    // --- MAIN DATA FETCHING LOGIC ---
    const fetchInitialData = async () => {
      // Set a general loading indicator
      setIsLoadingGrades(true);
      
      try {
        // 1. Get token once for all calls.
        const token = localStorage.getItem('token');
        if (!token) {
          setMessage("Authentication error: Not logged in.");
          return;
        }

        // 2. Define all API calls
        const gradesData = fetchFromApi('https://localhost:7025/api/GradeLevel/my-grade-levels', token);
        const patternsData = fetchFromApi('https://localhost:7025/api/InteractionPattern/my-patterns', token);
        const attitudesData = fetchFromApi('https://localhost:7025/api/AttitudeTemplate/my-templates', token);
        const objectivesData = fetchFromApi('https://localhost:7025/api/ObjectiveTemplate/my-templates', token);
         const preparationsData = fetchFromApi('https://localhost:7025/api/PreparationType/my-types', token);
        
        // 3. Wait for all fetches to complete
        const [
          grades,
          patterns,
          attitudes,
          objectives,
          preparations
        ] = await Promise.all([gradesData, patternsData, attitudesData, objectivesData,preparationsData]);

        // 4. Update all states with live data
        setGradeLevels(grades);
        setInteractionPatterns(patterns);
        setAttitudeTemplates(attitudes);
        setObjectiveTemplates(objectives);
        setPreparationTemplates(preparations);
        // Set remaining mock data (if any)
        setSkillTemplates(mockDatabase.skillTemplates);
        setMethodTemplates(mockDatabase.methodTemplates);

      } catch (error) {
        console.error("Failed to fetch initial data:", error);
        setMessage("Error: Could not load data from the server.");
      } finally {
        // Turn off the loading indicator
        setIsLoadingGrades(false);
      }
    };
    
    fetchInitialData();
  }, []);

  // ... (the second useEffect for fetching classes remains the same)
  useEffect(() => {
    if (!selectedGradeId) {
      setClasses([]);
      setSelectedClassId('');
      return;
    }
    const fetchClasses = async () => {
      setIsLoadingClasses(true);
      const result = await mockFetchClassesByGrade(selectedGradeId);
      setClasses(result.data || []);
      setIsLoadingClasses(false);
    };
    fetchClasses();
  }, [selectedGradeId]);

  // ... (all other functions like handleAddItem, generateLessonContent, etc. remain the same)
  const handleAddItem = (id, selectedItems, setSelectedItems, setItemToAdd) => {
    if (id && !selectedItems.includes(parseInt(id))) {
      setSelectedItems([...selectedItems, parseInt(id)]);
    }
    setItemToAdd('');
  };
  const handleRemoveItem = (id, selectedItems, setSelectedItems) => {
    setSelectedItems(selectedItems.filter(itemId => itemId !== id));
  };
  const generateLessonContent = () => {
    let html = `<div style="font-family: Arial, sans-serif; padding: 20px;">`;
    html += `<h1 style="text-align: center; color: #1f2937;">${lessonTitle || 'Lesson Plan'}</h1>`;
    html += `<div style="display: flex; justify-content: space-between; margin-bottom: 20px;">`;
    html += `<p><strong>Date of Preparation:</strong> ${dateOfPreparation || 'N/A'}</p>`;
    html += `<p><strong>Date of Teaching:</strong> ${dateOfTeaching || 'N/A'}</p>`;
    html += `</div>`;
    if (unitNumber || unitName) {
      html += `<h2 style="color: #2563eb;">${unitNumber ? unitNumber + '. ' : ''}${unitName}</h2>`;
    }
    if (lessonNumber) {
      html += `<h3 style="color: #4b5563;">${lessonNumber}</h3>`;
    }
    if (selectedObjectives.length > 0) {
      html += `<h3 style="color: #dc2626; margin-top: 20px;">A. Objectives:</h3>`;
      selectedObjectives.forEach((objId, idx) => {
        const template = objectiveTemplates.find(t => t.id === objId);
        if (template) {
          html += `<p><strong>${idx + 1}. ${template.name}:</strong> ${template.content}</p>`;
        }
      });
    }
    html += `<h3 style="color: #dc2626; margin-top: 20px;">B. Competences:</h3>`
    const hasLanguageFocus = languageFocus.some(lf => lf.content.trim());
    if (hasLanguageFocus) {
      html += `<p><strong>1. Language focus:</strong></p><ul style="list-style-position: inside; padding-left: 10px;">`;
      languageFocus.forEach(lf => {
        if (lf.content.trim()) {
          html += `<li><strong>${lf.type}:</strong> ${lf.content}</li>`;
        }
      });
      html += `</ul>`;
    }
    if (selectedSkills.length > 0) {
      html += `<p><strong>2. Skills:</strong></p><ul style="list-style-position: inside; padding-left: 10px;">`;
      selectedSkills.forEach(skillId => {
        const skill = skillTemplates.find(t => t.id === skillId);
        if (skill) {
          html += `<li><strong>${skill.skillType}:</strong> ${skill.description}</li>`;
        }
      });
        html += `</ul>`;
    }
    if (selectedAttitudes.length > 0) {
      html += `<p><strong>3. Attitudes:</strong></p><ul style="list-style-position: inside; padding-left: 10px;">`;
      selectedAttitudes.forEach(attId => {
        const att = attitudeTemplates.find(t => t.id === attId);
        if (att) {
          html += `<li>${att.content}</li>`;
        }
      });
      html += `</ul>`;
    }
    html += `<h3 style="color: #dc2626; margin-top: 20px;">C. Preparations:</h3>`;
    // Use 'selectedPreparations' instead of 'preparations'
    selectedPreparations.forEach(prep => { 
      if (prep.materials.trim()) {
        // Use 'prep.name' instead of 'prep.type'
        html += `<p>- <strong>${prep.name}:</strong> ${prep.materials}</p>`;
      }
    });
    if (selectedMethod) {
      const method = methodTemplates.find(m => m.id === parseInt(selectedMethod));
      if (method) {
        html += `<h3 style="color: #dc2626; margin-top: 20px;">D. Methods:</h3>`;
        html += `<p>- ${method.name}</p>`;
      }
    }
    if (activities.length > 0) {
        html += `<h3 style="color: #dc2626; margin-top: 20px;">E. Procedures:</h3>`;
        html += `<table style="width: 100%; border-collapse: collapse; margin-top: 10px;">`;
        html += `<thead><tr style="background-color: #fef08a;">
                <th style="border: 1px solid #000; padding: 8px;">Time/Stages</th>
                <th style="border: 1px solid #000; padding: 8px;">Activities</th>
                <th style="border: 1px solid #000; padding: 8px;">Interactions</th>
            </tr></thead><tbody>`;
        activities.forEach(stage => {
            if (stage.subActivities.length > 0) {
                stage.subActivities.forEach((subActivity, subIndex) => {
                    const pattern = interactionPatterns.find(p => p.id === parseInt(subActivity.interactionPatternId));
                    html += `<tr>`;
                    if (subIndex === 0) {
                        const totalTime = stage.subActivities.reduce((sum, act) => sum + (act.timeInMinutes || 0), 0);
                        html += `<td style="border: 1px solid #000; padding: 8px; vertical-align: top;" rowspan="${stage.subActivities.length}">
                            <b>${stage.stageName}</b><br/>${totalTime} minutes
                        </td>`;
                    }
                    html += `<td style="border: 1px solid #000; padding: 8px; vertical-align: top;">${subActivity.content.replace(/\n/g, '<br/>')}</td>`;
                    html += `<td style="border: 1px solid #000; padding: 8px; vertical-align: top; text-align: center;">${pattern ? pattern.shortCode : ''}</td>`;
                    html += `</tr>`;
                });
            }
        });
        html += `</tbody></table>`;
    }
    html += `</div>`;
    if (contentRef.current) {
      contentRef.current.innerHTML = html;
    }
  };
  const handleNextStep = () => {
    if (currentStep === totalSteps) {
      generateLessonContent();
    }
    if (currentStep < totalSteps) {
      setCurrentStep(currentStep + 1);
    }
  };
  const handlePrevStep = () => {
    if (currentStep > 1) {
      setCurrentStep(currentStep - 1);
    }
  };
  const handleSaveLesson = async () => {
    setMessage('Saving lesson...');
    const saveRequest = {
      title: lessonTitle,
      description: lessonDescription,
      content: contentRef.current?.innerHTML || '',
      classId: parseInt(selectedClassId),
      dateOfPreparation,
      dateOfTeaching,
      unitNumber,
      unitName,
      lessonNumber,
      objectives: selectedObjectives,
      skills: selectedSkills,
      attitudes: selectedAttitudes,
      languageFocus: languageFocus.filter(lf => lf.content.trim()),
      preparations: selectedPreparations.map(p => ({ type: p.name, materials: p.materials })).filter(p => p.materials.trim()),
      methodId: selectedMethod ? parseInt(selectedMethod) : null,
      activities: activities,
    };
    try {
      const result = await mockSaveLesson(saveRequest);
      if (result.success) {
        setMessage(`Lesson saved successfully! Mock ID: ${result.data.id}`);
      }
    } catch (error) {
      setMessage('Failed to save lesson: ' + error.message);
    }
  };

  // ... (all JSX rendering code from return() is unchanged)
  const RichTextToolbar = () => ( <div className="flex flex-wrap items-center p-3 border-b bg-gray-100 rounded-t-xl sticky top-0 z-10"> <div className="flex items-center space-x-1 border-r pr-3 flex-wrap gap-1"> {[{ Icon: Bold, cmd: 'bold', title: 'Bold' }, { Icon: Italic, cmd: 'italic', title: 'Italic' }, { Icon: Underline, cmd: 'underline', title: 'Underline' }, ].map(({ Icon, cmd, title }) => ( <button key={cmd} className="p-1.5 hover:bg-gray-300 rounded text-gray-700 transition" onClick={() => document.execCommand(cmd)} title={title}> <Icon className="w-4 h-4" /> </button> ))} </div> <button className="ml-auto flex items-center space-x-2 px-3 py-1.5 text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition shadow" onClick={handleSaveLesson}> <Save className="w-4 h-4" /> <span className="text-sm font-medium">Save Lesson</span> </button> </div> );
  const StepIndicator = () => ( <div className="flex items-center justify-center mb-8"> {[1, 2, 3, 4, 5, 6].map((step) => ( <React.Fragment key={step}> <div className={`flex flex-col items-center cursor-pointer ${step <= currentStep ? 'text-blue-600' : 'text-gray-400'}`} onClick={() => setCurrentStep(step)}> <div className={`w-10 h-10 rounded-full flex items-center justify-center font-bold transition-all duration-300 ${step < currentStep ? 'bg-green-500 text-white' : step === currentStep ? 'bg-blue-600 text-white scale-110' : 'bg-gray-300'}`}> {step < currentStep ? <CheckCircle2 className="w-6 h-6" /> : step} </div> <span className="text-xs mt-1 hidden sm:block"> {['Basic', 'Objectives', 'Skills', 'Attitudes', 'Prep', 'Activities'][step - 1]} </span> </div> {step < 6 && <div className={`h-1 w-12 sm:w-20 transition-all duration-500 ${step < currentStep ? 'bg-green-500' : 'bg-gray-300'}`} />} </React.Fragment> ))} </div> );
  const ItemSelector = ({ title, templates, selectedIds, onAdd, onRemove, selectedValue, onSelectChange }) => { const availableItems = templates.filter(t => !selectedIds.includes(t.id)); return ( <div className="space-y-4"> <div className="flex items-center gap-2"> <select value={selectedValue} onChange={onSelectChange} className="w-full p-2 border border-gray-300 rounded-lg text-sm" disabled={availableItems.length === 0} > <option value="">{availableItems.length > 0 ? `Select a ${title.toLowerCase()}...` : `All ${title.toLowerCase()} added`}</option> {availableItems.map(item => ( <option key={item.id} value={item.id}>{item.name || item.skillType}</option> ))} </select> <button onClick={() => onAdd(selectedValue)} disabled={!selectedValue} className="px-3 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:bg-gray-300 disabled:cursor-not-allowed" > <Plus className="w-5 h-5" /> </button> </div> <div className="space-y-2"> {selectedIds.length > 0 ? ( selectedIds.map(id => { const item = templates.find(t => t.id === id); if (!item) return null; return ( <div key={id} className="flex items-center justify-between p-2 bg-gray-100 border rounded-lg animate-fade-in"> <div className="text-sm"> <p className="font-semibold">{item.name || item.skillType}</p> <p className="text-gray-600">{item.content || item.description}</p> </div> <button onClick={() => onRemove(id)} className="text-red-500 hover:text-red-700 p-1"> <X className="w-4 h-4" /> </button> </div> ); }) ) : ( <p className="text-sm text-gray-500 text-center py-2">No {title.toLowerCase()} added yet.</p> )} </div> </div> ); }

  return (
    <div className="min-h-screen bg-gray-50 p-4 sm:p-8 font-['Inter']">
      <header className="mb-8">
        <h1 className="text-3xl font-extrabold text-gray-900 flex items-center">
          <BookOpen className="w-8 h-8 mr-3 text-red-600" />
          Create Lesson Plan
        </h1>
        <p className="text-gray-500 mt-1">Build your lesson plan step by step, or jump to any step.</p>
      </header>

      {message && (
        <div className="mb-4 p-3 bg-indigo-100 text-indigo-800 rounded-lg shadow-sm text-sm font-medium animate-fade-in">
          {message}
        </div>
      )}

      <StepIndicator />

      <div className="flex flex-col lg:flex-row gap-8">
        <div className="lg:w-1/3 p-6 bg-white rounded-xl shadow-2xl h-fit sticky top-4">
          
          {currentStep < 6 && (
            <>
              {currentStep === 1 && (
                   <div className="space-y-4">
                     <div className="flex items-center space-x-2 text-blue-600 mb-4">
                       <Calendar className="w-6 h-6" />
                       <h2 className="text-xl font-bold">Basic Information</h2>
                     </div>
   
                     <div>
                       <label className="block text-sm font-bold text-gray-700 mb-2">Grade Level</label>
                       <select value={selectedGradeId} onChange={(e) => setSelectedGradeId(e.target.value)} disabled={isLoadingGrades} className="w-full p-3 border border-gray-300 rounded-lg">
                         <option value="">{isLoadingGrades ? 'Loading...' : 'Select Grade'}</option>
                         {gradeLevels.map(grade => <option key={grade.id} value={grade.id}>{grade.name}</option>)}
                       </select>
                     </div>
   
                     <div>
                       <label className="block text-sm font-bold text-gray-700 mb-2">Class</label>
                       <select value={selectedClassId} onChange={(e) => setSelectedClassId(e.target.value)} disabled={!selectedGradeId || isLoadingClasses} className="w-full p-3 border border-gray-300 rounded-lg">
                         <option value="">{isLoadingClasses ? 'Loading...' : !selectedGradeId ? 'Select grade first' : 'Select Class'}</option>
                         {classes.map(cls => <option key={cls.id} value={cls.id}>{cls.name}</option>)}
                       </select>
                     </div>
                     
                     <div className="grid grid-cols-2 gap-3">
                         <div>
                             <label className="block text-sm font-bold text-gray-700 mb-2">Date of Preparation</label>
                             <input type="date" value={dateOfPreparation} onChange={(e) => setDateOfPreparation(e.target.value)} className="w-full p-2 border border-gray-300 rounded-lg" />
                         </div>
                         <div>
                             <label className="block text-sm font-bold text-gray-700 mb-2">Date of Teaching</label>
                             <input type="date" value={dateOfTeaching} onChange={(e) => setDateOfTeaching(e.target.value)} className="w-full p-2 border border-gray-300 rounded-lg" />
                         </div>
                     </div>
   
                     <div className="grid grid-cols-2 gap-3">
                         <div>
                             <label className="block text-sm font-bold text-gray-700 mb-2">Unit Number</label>
                             <input type="text" placeholder="e.g., Unit 6" value={unitNumber} onChange={(e) => setUnitNumber(e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg" />
                         </div>
                         <div>
                             <label className="block text-sm font-bold text-gray-700 mb-2">Lesson Number</label>
                             <input type="text" placeholder="e.g., TIẾT 55" value={lessonNumber} onChange={(e) => setLessonNumber(e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg" />
                         </div>
                     </div>
   
                    <div>
                      <label className="block text-sm font-bold text-gray-700 mb-2">Unit Name</label>
                      <input type="text" placeholder="e.g., Endangered Species" value={unitName} onChange={(e) => setUnitName(e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg" />
                    </div>
                    
                    <div>
                      <label className="block text-sm font-bold text-gray-700 mb-2">Lesson Title</label>
                      <input type="text" placeholder="e.g., Getting Started" value={lessonTitle} onChange={(e) => setLessonTitle(e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg" />
                    </div>
   
                    <div>
                      <label className="block text-sm font-bold text-gray-700 mb-2">Lesson Description</label>
                      <textarea rows="3" placeholder="Brief description of the lesson" value={lessonDescription} onChange={(e) => setLessonDescription(e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg" />
                    </div>
                  </div>
              )}
              {currentStep === 2 && (
                  <div className="space-y-4">
                      <div className="flex items-center space-x-2 text-blue-600 mb-4">
                          <Target className="w-6 h-6" />
                          <h2 className="text-xl font-bold">Learning Objectives</h2>
                      </div>
                      <ItemSelector
                          title="Objective"
                          templates={objectiveTemplates}
                          selectedIds={selectedObjectives}
                          onAdd={(id) => handleAddItem(id, selectedObjectives, setSelectedObjectives, setObjectiveToAdd)}
                          onRemove={(id) => handleRemoveItem(id, selectedObjectives, setSelectedObjectives)}
                          selectedValue={objectiveToAdd}
                          onSelectChange={(e) => setObjectiveToAdd(e.target.value)}
                      />
                  </div>
              )}
              {currentStep === 3 && (
                  <div className="space-y-4">
                      <div className="flex items-center space-x-2 text-blue-600 mb-4">
                          <Brain className="w-6 h-6" />
                          <h2 className="text-xl font-bold">Skills Development</h2>
                      </div>
                      <ItemSelector
                          title="Skill"
                          templates={skillTemplates}
                          selectedIds={selectedSkills}
                          onAdd={(id) => handleAddItem(id, selectedSkills, setSelectedSkills, setSkillToAdd)}
                          onRemove={(id) => handleRemoveItem(id, selectedSkills, setSelectedSkills)}
                          selectedValue={skillToAdd}
                          onSelectChange={(e) => setSkillToAdd(e.target.value)}
                      />
                  </div>
              )}
              {currentStep === 4 && (
                  <div className="space-y-6">
                      <div className="flex items-center space-x-2 text-blue-600 mb-2">
                          <Heart className="w-6 h-6" />
                          <h2 className="text-xl font-bold">Attitudes & Language</h2>
                      </div>
                      
                      <div>
                          <h3 className="font-bold text-gray-700 mb-3">Attitudes</h3>
                          <ItemSelector
                              title="Attitude"
                              templates={attitudeTemplates}
                              selectedIds={selectedAttitudes}
                              onAdd={(id) => handleAddItem(id, selectedAttitudes, setSelectedAttitudes, setAttitudeToAdd)}
                              onRemove={(id) => handleRemoveItem(id, selectedAttitudes, setSelectedAttitudes)}
                              selectedValue={attitudeToAdd}
                              onSelectChange={(e) => setAttitudeToAdd(e.target.value)}
                          />
                      </div>
   
                      <div className="mt-6">
                          <h3 className="font-bold text-gray-700 mb-3">Language Focus</h3>
                          {languageFocus.map((lf, idx) => (
                              <div key={idx} className="mb-3">
                                  <label className="block text-sm font-semibold text-gray-600 mb-1">{lf.type}</label>
                                  <textarea rows="2" placeholder={`Enter ${lf.type.toLowerCase()} details...`} value={lf.content} onChange={(e) => {
                                      const updated = [...languageFocus];
                                      updated[idx].content = e.target.value;
                                      setLanguageFocus(updated);
                                  }} className="w-full p-2 border border-gray-300 rounded-lg text-sm" />
                              </div>
                          ))}
                      </div>
                  </div>
              )}
{currentStep === 5 && (
    <div className="space-y-6">
      <div className="flex items-center space-x-2 text-blue-600 mb-2">
        <Book className="w-6 h-6" />
        <h2 className="text-xl font-bold">Preparations & Methods</h2>
      </div>
      
      <div>
        <h3 className="font-bold text-gray-700 mb-3">Preparations</h3>
        <div className="space-y-4">
            {/* Dropdown for adding new preparations */}
            <div className="flex items-center gap-2">
                <select
                    value={preparationToAdd}
                    onChange={(e) => setPreparationToAdd(e.target.value)}
                    className="w-full p-2 border border-gray-300 rounded-lg text-sm"
                >
                    <option value="">Select a preparation type...</option>
                    {preparationTemplates
                        .filter(t => !selectedPreparations.some(p => p.id === t.id))
                        .map(template => (
                            <option key={template.id} value={template.id}>{template.name}</option>
                        ))
                    }
                </select>
                <button
                    onClick={() => handleAddPreparation(preparationToAdd)}
                    disabled={!preparationToAdd}
                    className="px-3 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:bg-gray-300"
                >
                    <Plus className="w-5 h-5" />
                </button>
            </div>

            {/* List of selected preparations (NOW NON-EDITABLE) */}
            <div className="space-y-3">
                {selectedPreparations.map((prep) => (
                  <div key={prep.id} className="p-3 bg-gray-50 border rounded-lg">
                    <div className="flex justify-between items-center mb-1">
                        <label className="block text-sm font-semibold text-gray-700">{prep.name}</label>
                        <button onClick={() => handleRemovePreparation(prep.id)} className="text-red-500 hover:text-red-700 p-1">
                            <X className="w-4 h-4" />
                        </button>
                    </div>
                    {/* The textarea is replaced with this p tag */}
                    <p className="text-sm text-gray-800 p-2 bg-white border rounded-md whitespace-pre-wrap">
                        {prep.materials}
                    </p>
                  </div>
                ))}
            </div>
        </div>
      </div>

      <div className="mt-6">
        <h3 className="font-bold text-gray-700 mb-3">Teaching Method</h3>
        <select value={selectedMethod} onChange={(e) => setSelectedMethod(e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg">
          <option value="">Select a teaching method</option>
          {methodTemplates.map(method => (
            <option key={method.id} value={method.id}>{method.name}</option>
          ))}
        </select>
        {selectedMethod && (
          <p className="text-sm text-gray-600 mt-2 p-2 bg-gray-50 rounded-md">
            {methodTemplates.find(m => m.id === parseInt(selectedMethod))?.description}
          </p>
        )}
      </div>
    </div>
  )}
            </>
          )}

          {currentStep === 6 && (
            <div className="space-y-4">
              <div className="flex items-center space-x-2 text-blue-600 mb-4">
                <Users className="w-6 h-6" />
                <h2 className="text-xl font-bold">Lesson Activities</h2>
              </div>
              
              <div className="space-y-6 max-h-[60vh] overflow-y-auto pr-2">
                {activities.map((stage, stageIndex) => (
                  <div key={stageIndex} className="p-4 border-2 border-gray-200 rounded-lg bg-gray-50 space-y-4">
                    <div className="flex justify-between items-center pb-2 border-b">
                      <input 
                        type="text"
                        value={stage.stageName}
                        onChange={(e) => updateStageName(stageIndex, e.target.value)}
                        className="font-bold text-lg text-gray-800 p-1 rounded bg-transparent focus:bg-white"
                      />
                       <button onClick={() => removeStage(stageIndex)} className="text-red-500 hover:text-red-700 p-1 rounded-full hover:bg-red-100">
                         <X className="w-4 h-4" />
                       </button>
                    </div>

                    {stage.subActivities.map((sub, subIndex) => (
                      <div key={subIndex} className="p-3 border rounded-md bg-white relative">
                        <span className="absolute -left-2 top-2 text-xs bg-blue-500 text-white font-bold rounded-full h-5 w-5 flex items-center justify-center">{subIndex + 1}</span>
                        <div className="space-y-2 pl-4">
                            <div>
                                <label className="block text-xs font-semibold text-gray-600 mb-1">Time (minutes)</label>
                                <input type="number" min="0" value={sub.timeInMinutes} onChange={(e) => updateSubActivity(stageIndex, subIndex, 'timeInMinutes', parseInt(e.target.value) || 0)} className="w-full p-2 border border-gray-300 rounded text-sm" />
                            </div>
                            <div>
                                <label className="block text-xs font-semibold text-gray-600 mb-1">Activities</label>
                                <textarea rows="3" placeholder="Describe the activities..." value={sub.content} onChange={(e) => updateSubActivity(stageIndex, subIndex, 'content', e.target.value)} className="w-full p-2 border border-gray-300 rounded text-sm" />
                            </div>
                            <div>
                                <label className="block text-xs font-semibold text-gray-600 mb-1">Interaction Pattern</label>
                                <select value={sub.interactionPatternId} onChange={(e) => updateSubActivity(stageIndex, subIndex, 'interactionPatternId', e.target.value)} className="w-full p-2 border border-gray-300 rounded text-sm">
                                  <option value="">Select interaction</option>
                                  {interactionPatterns.map(pattern => (
                                    <option key={pattern.id} value={pattern.id}>{pattern.name} ({pattern.shortCode})</option>
                                  ))}
                                </select>
                            </div>
                        </div>
                         {stage.subActivities.length > 0 && (
                           <button onClick={() => removeSubActivity(stageIndex, subIndex)} className="absolute top-1 right-1 text-gray-400 hover:text-red-600">
                             <X className="w-3 h-3" />
                           </button>
                         )}
                      </div>
                    ))}
                    <button onClick={() => addSubActivity(stageIndex)} className="w-full text-xs py-1 px-2 bg-blue-100 text-blue-800 rounded hover:bg-blue-200 flex items-center justify-center space-x-1">
                      <Plus className="w-3 h-3" />
                      <span>Add Activity to this Stage</span>
                    </button>
                  </div>
                ))}
              </div>
              
              <button onClick={addStage} className="w-full py-2 px-4 bg-green-600 text-white rounded-lg hover:bg-green-700 flex items-center justify-center space-x-2">
                <Plus className="w-4 h-4" />
                <span>Add New Stage</span>
              </button>
            </div>
          )}

          <div className="flex justify-between mt-6 pt-4 border-t">
            <button onClick={handlePrevStep} disabled={currentStep === 1} className={`flex items-center space-x-2 px-4 py-2 rounded-lg transition ${currentStep === 1 ? 'bg-gray-300 text-gray-500 cursor-not-allowed' : 'bg-gray-600 text-white hover:bg-gray-700'}`}>
              <ChevronLeft className="w-4 h-4" />
              <span>Previous</span>
            </button>
            
            <button onClick={handleNextStep} className="flex items-center space-x-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 shadow-md">
              <span>{currentStep === totalSteps ? 'Generate' : 'Next'}</span>
              <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </div>

        <div className="lg:w-2/3 flex flex-col">
          <div className="bg-white rounded-xl shadow-2xl overflow-hidden">
            <RichTextToolbar />
            <div ref={contentRef} contentEditable={false} suppressContentEditableWarning={true} className="p-6 h-[70vh] overflow-y-auto outline-none prose prose-sm max-w-none text-gray-800 focus:ring-2 focus:ring-red-500" style={{ wordWrap: 'break-word', overflowWrap: 'break-word' }}>
              <div className="text-center text-gray-400 py-12">
                <BookOpen className="w-16 h-16 mx-auto mb-4 opacity-50" />
                <p className="text-lg">Complete the steps to generate your lesson plan</p>
                <p className="text-sm mt-2">The preview will appear here. You can edit it directly.</p>
              </div>
            </div>
          </div>
          
          <div className="mt-6 flex justify-end space-x-4 flex-wrap gap-2">
            <button onClick={() => {
              if (contentRef.current) {
                navigator.clipboard.writeText(contentRef.current.innerText);
                setMessage('Lesson content (plain text) copied to clipboard!');
              }
            }} className="flex items-center space-x-2 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition shadow-md">
              <Copy className="w-4 h-4" />
              <span>Copy text</span>
            </button>
            <button onClick={() => {
              if (contentRef.current) {
                const htmlContent = contentRef.current.innerHTML;
                const title = lessonTitle || 'Lesson_Plan';
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
                setMessage('Lesson plan downloaded as .doc!');
              }
            }} className="flex items-center space-x-2 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition shadow-md">
              <Download className="w-4 h-4" />
              <span>Download .doc</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}