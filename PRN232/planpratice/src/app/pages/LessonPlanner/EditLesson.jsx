import React, { useState, useRef, useEffect, act } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
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
  AlertTriangle,
} from 'lucide-react';

// =================================================================
// API CONFIGURATION
// =================================================================
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

// =================================================================
// API HELPER FUNCTIONS
// =================================================================
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
    throw new Error(`API request to ${endpoint} failed with status ${response.status}`);
  }
  const result = await response.json();
  if (result.success && result.data !== undefined) {
    return result.data;
  }
  throw new Error(`Invalid data format from ${endpoint}`);
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
    throw new Error(errorData.message || `API request failed with status ${response.status}`);
  }
  return response.json(); // Assuming the PUT response might not have a full body but we check for success
};


export default function EditLesson() {
  const { id: lessonId } = useParams();
  const navigate = useNavigate();

  // Component State
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [message, setMessage] = useState('');
  const [currentStep, setCurrentStep] = useState(1);
  const totalSteps = 6;
  const contentRef = useRef(null);

  // Form State (same as CreateLesson)
  const [gradeLevels, setGradeLevels] = useState([]);
  const [allClasses, setAllClasses] = useState([]);
  const [classes, setClasses] = useState([]);
  const [selectedGradeId, setSelectedGradeId] = useState('');
  const [selectedClassId, setSelectedClassId] = useState('');
  const [lessonTitle, setLessonTitle] = useState('');
  const [lessonDescription, setLessonDescription] = useState('');
  const [dateOfPreparation, setDateOfPreparation] = useState('');
  const [dateOfTeaching, setDateOfTeaching] = useState('');
  const [unitNumber, setUnitNumber] = useState('');
  const [unitName, setUnitName] = useState('');
  const [lessonNumber, setLessonNumber] = useState('');
  const [objectiveTemplates, setObjectiveTemplates] = useState([]);
  const [selectedObjectives, setSelectedObjectives] = useState([]);
  const [objectiveToAdd, setObjectiveToAdd] = useState('');
  const [skillTemplates, setSkillTemplates] = useState([]);
  const [selectedSkills, setSelectedSkills] = useState([]);
  const [skillToAdd, setSkillToAdd] = useState('');
  const [attitudeTemplates, setAttitudeTemplates] = useState([]);
  const [selectedAttitudes, setSelectedAttitudes] = useState([]);
  const [attitudeToAdd, setAttitudeToAdd] = useState('');
  const [languageFocusTypes, setLanguageFocusTypes] = useState([]);
  const [languageFocus, setLanguageFocus] = useState([{ typeId: null, content: '' }]);
  const [preparationTemplates, setPreparationTemplates] = useState([]);
  const [selectedPreparations, setSelectedPreparations] = useState([]);
  const [preparationToAdd, setPreparationToAdd] = useState('');
  const [methodTemplates, setMethodTemplates] = useState([]);
  const [selectedMethod, setSelectedMethod] = useState('');
  const [activityTemplates, setActivityTemplates] = useState([]);
  const [interactionPatterns, setInteractionPatterns] = useState([]);
  const [activities, setActivities] = useState([]);

  // =================================================================
  // DATA FETCHING: Load all templates AND the specific lesson plan
  // =================================================================
  useEffect(() => {
    const fetchAllDataForEdit = async () => {
      if (!lessonId) {
        setError("No lesson ID provided.");
        setIsLoading(false);
        return;
      }
      setIsLoading(true);
      try {
        const token = getAuthToken();
        const [
          gradesData,
          patternsData,
          attitudesData,
          objectivesData,
          preparationsData,
          skillsData,
          languageTypesData,
          allClassesData,
          methodsData,
          activityTempsData,
          lessonToEdit
        ] = await Promise.all([
          fetchFromApi('/Grade/my-grade-levels', token),
          fetchFromApi('/Interaction/pattern/my-patterns', token),
          fetchFromApi('/Attitude/template/my-templates', token),
          fetchFromApi('/Objective/template/my-templates', token),
          fetchFromApi('/Preparation/my-types', token),
          fetchFromApi('/Skill/template/my-templates', token),
          fetchFromApi('/Language/my-types', token),
          fetchFromApi('/Class/my-classes', token),
          fetchFromApi('/Method/template/my-templates', token),
          fetchFromApi('/Activity/template/my-templates', token),
          fetchFromApi(`/Lesson/planner/${lessonId}`, token)
        ]);

        // 1. Set all template states
        setGradeLevels(gradesData);
        setAllClasses(allClassesData);

        // 2. Process custom items (with null template IDs) and add them to template arrays
        const customObjectives = lessonToEdit.objectives?.filter(o => !o.objectiveTemplateId).map((o, idx) => ({
          id: `custom_obj_${idx}`,
          name: o.name || 'Custom Objective',
          content: o.content || '',
          isCustom: true
        })) || [];

        const customSkills = lessonToEdit.skills?.filter(s => !s.skillTemplateId).map((s, idx) => ({
          id: `custom_skill_${idx}`,
          name: s.name || 'Custom Skill',
          description: s.description || '',
          isCustom: true
        })) || [];

        const customAttitudes = lessonToEdit.attitudes?.filter(a => !a.attitudeTemplateId).map((a, idx) => ({
          id: `custom_att_${idx}`,
          name: a.name || 'Custom Attitude',
          content: a.customContent || '',
          isCustom: true
        })) || [];

        const customLanguageFocusItems = lessonToEdit.languageFocusItems?.filter(lf => !lf.languageFocusTypeId).map((lf, idx) => ({
          id: `custom_lf_${idx}`,
          name: lf.name,
          content: lf.content || '',
          isCustom: true
        })) || [];

        const customPreparations = lessonToEdit.preparations?.filter(p => !p.preparationTypeId).map((p, idx) => ({
          id: `custom_prep_${idx}`,
          name: p.name || 'Custom Preparation',
          description: p.materials || '',
          isCustom: true
        })) || [];
        
        const customInteractionPatterns = [];
        const customActivities = [];
        lessonToEdit.activityStages?.forEach((stage, stageIdx) => {
          stage.activityItems.forEach((item, itemIdx) => {
            if (!item.activityTemplateId && item.content) {
              customActivities.push({
                id: `custom_act_${stageIdx}_${itemIdx}`,
                name: stage.stageName,
                content: item.content,
                isCustom: true
              });
            }
            if (!item.interactionPatternId && item.interactionPatternName) {
              customInteractionPatterns.push({
                id: `custom_ip_${stageIdx}_${itemIdx}`,
                name: item.interactionPatternName,
                shortCode: item.interactionPatternShortCode,
                description: '',
                isCustom: true
              });
            }
          });
        });

        if (!lessonToEdit.methodTemplateId) {
          methodsData.push({
            id: 'custom_method_0',
            name: lessonToEdit.methodName || 'Custom Method',
            description: lessonToEdit.methodDescription || '',
            isCustom: true
          });
        }

        // Merge custom items with template data
        setObjectiveTemplates([...objectivesData, ...customObjectives]);
        setSkillTemplates([...skillsData, ...customSkills]);
        setAttitudeTemplates([...attitudesData, ...customAttitudes]);
        setPreparationTemplates([...preparationsData, ...customPreparations]);
        setInteractionPatterns([...patternsData, ...customInteractionPatterns]);
        setActivityTemplates([...activityTempsData, ...customActivities]);
        setLanguageFocusTypes([...languageTypesData, ...customLanguageFocusItems]);
        setMethodTemplates(methodsData);

        // 3. Populate form state from the fetched lesson plan (`lessonToEdit`)
        setLessonTitle(lessonToEdit.title || '');
        setLessonDescription(lessonToEdit.description || '');
        setDateOfPreparation(lessonToEdit.dateOfPreparation ? new Date(lessonToEdit.dateOfPreparation).toISOString().split('T')[0] : '');
        setDateOfTeaching(lessonToEdit.dateOfTeaching ? new Date(lessonToEdit.dateOfTeaching).toISOString().split('T')[0] : '');
        setLessonNumber(lessonToEdit.lessonNumber || '');
        setUnitNumber(lessonToEdit.unitNumber || '');
        setUnitName(lessonToEdit.unitName || '');
        // Auto select custom method
        setSelectedMethod(lessonToEdit.methodTemplateId || 'custom_method_0');

        // Set grade and class
        const lessonClass = allClassesData.find(c => c.id === lessonToEdit.classId);
        if (lessonClass) {
          setSelectedGradeId(lessonClass.gradeLevelId);
          const filteredClasses = allClassesData.filter(c => c.gradeLevelId === lessonClass.gradeLevelId);
          setClasses(filteredClasses);
          setSelectedClassId(lessonToEdit.classId);
        }

        // Populate selections (including custom items)
        const objectiveIds = lessonToEdit.objectives?.map((o, idx) =>
          o.objectiveTemplateId || `custom_obj_${lessonToEdit.objectives.filter(obj => !obj.objectiveTemplateId).indexOf(o)}`
        ).filter(Boolean) || [];

        const skillIds = lessonToEdit.skills?.map((s, idx) =>
          s.skillTemplateId || `custom_skill_${lessonToEdit.skills.filter(sk => !sk.skillTemplateId).indexOf(s)}`
        ).filter(Boolean) || [];

        const attitudeIds = lessonToEdit.attitudes?.map((a, idx) =>
          a.attitudeTemplateId || `custom_att_${lessonToEdit.attitudes.filter(att => !att.attitudeTemplateId).indexOf(a)}`
        ).filter(Boolean) || [];

        setSelectedObjectives(objectiveIds);
        setSelectedSkills(skillIds);
        setSelectedAttitudes(attitudeIds);
        // Map languageFocusTypeId from backend to typeId for frontend
        setLanguageFocus(lessonToEdit.languageFocusItems?.length > 0
          ? lessonToEdit.languageFocusItems.map((lf, idx) => ({
            // create the same mapping id logic
            typeId: lf.languageFocusTypeId ?? `custom_lf_${idx}`,
            content: lf.content || ''
          }))
          : [{ typeId: null, content: '' }]);

        // Reconstruct preparations with name from template (including custom items)
        const reconstructedPreparations = lessonToEdit.preparations?.map((prep, idx) => {
          if (prep.preparationTypeId) {
            const template = preparationsData.find(t => t.id === prep.preparationTypeId);
            return {
              id: prep.preparationTypeId,
              name: template?.name || prep.name || 'Unknown Type',
              materials: prep.materials,
            };
          } else {
            // Custom preparation without template
            return {
              id: `custom_prep_${lessonToEdit.preparations.filter(p => !p.preparationTypeId).indexOf(prep)}`,
              name: prep.name || 'Custom Preparation',
              materials: prep.materials || '',
            };
          }
        }) || [];
        setSelectedPreparations(reconstructedPreparations);

        // Reconstruct activities structure for the UI (including custom activities)
        const reconstructedActivities = lessonToEdit.activityStages?.map((stage, stageIdx) => ({
          stageName: stage.stageName,
          subActivities: stage.activityItems.map((item, itemIdx) => ({
            timeInMinutes: item.timeInMinutes,
            activityTemplateId: item.activityTemplateId || (item.content ? `custom_act_${stageIdx}_${itemIdx}` : ''),
            interactionPatternId: item.interactionPatternId || '',
          }))
        })) || [];
        setActivities(reconstructedActivities);

        setMessage('✅ Lesson data loaded successfully. You can now edit the plan.');
      } catch (err) {
        console.error("Failed to fetch lesson data:", err);
        setError(`Failed to load lesson plan: ${err.message}`);
      } finally {
        setIsLoading(false);
      }
    };

    fetchAllDataForEdit();
  }, [lessonId, navigate]);

  // Effect to filter classes when grade changes
  useEffect(() => {
    if (selectedGradeId) {
      setClasses(allClasses.filter(c => c.gradeLevelId === parseInt(selectedGradeId)));
    } else {
      setClasses([]);
    }
  }, [selectedGradeId, allClasses]);


  // =================================================================
  // ALL HANDLER FUNCTIONS (mostly unchanged from CreateLesson)
  // =================================================================

  const handleAddItem = (id, selectedItems, setSelectedItems, setItemToAdd) => {
    if (id) {
      // Handle both numeric IDs (templates) and string IDs (custom items)
      const itemId = isNaN(id) ? id : parseInt(id);
      if (!selectedItems.includes(itemId)) {
        setSelectedItems([...selectedItems, itemId]);
      }
    }
    setItemToAdd('');
  };

  const handleRemoveItem = (id, selectedItems, setSelectedItems) => {
    setSelectedItems(selectedItems.filter(itemId => itemId !== id));
  };

  const handleAddPreparation = (templateId) => {
    if (!templateId) return;

    // Handle both numeric IDs (templates) and string IDs (custom items)
    const itemId = isNaN(templateId) ? templateId : parseInt(templateId);
    if (selectedPreparations.some(p => p.id === itemId)) return;

    const template = preparationTemplates.find(t => t.id === itemId);
    if (template) {
      setSelectedPreparations([
        ...selectedPreparations,
        { id: template.id, name: template.name, materials: template.description || '' }
      ]);
    }
    setPreparationToAdd('');
  };

  const handleRemovePreparation = (templateId) => {
    setSelectedPreparations(selectedPreparations.filter(p => p.id !== templateId));
  };

  const addStage = () => {
    setActivities([...activities, { stageName: 'New Stage', subActivities: [{ timeInMinutes: 5, activityTemplateId: '', interactionPatternId: '' }] }]);
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
    updatedActivities[stageIndex].subActivities.push({ timeInMinutes: 5, activityTemplateId: '', interactionPatternId: '' });
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

  // =================================================================
  // UPDATE LESSON (SAVE CHANGES)
  // =================================================================
  const handleUpdateLesson = async () => {
    setMessage('Updating lesson...');
    generateLessonContent(); // Ensure content is up-to-date before saving

    const updateRequest = {
      title: lessonTitle,
      description: lessonDescription,
      content: contentRef.current?.innerHTML || '',
      classId: parseInt(selectedClassId),
      dateOfPreparation: dateOfPreparation ? new Date(dateOfPreparation).toISOString() : null,
      dateOfTeaching: dateOfTeaching ? new Date(dateOfTeaching).toISOString() : null,
      lessonNumber: lessonNumber,
      unitNumber: unitNumber,
      unitName: unitName,
      unitId: null, // This can be extended if unit management is linked
      lessonDefinitionId: null, // Same as above
      ...((typeof selectedMethod === 'string' && selectedMethod.startsWith('custom_')) ? {
        methodTemplateId: null,
        methodName: methodTemplates.find(m => m.id === selectedMethod)?.name,
        methodDescription: methodTemplates.find(m => m.id === selectedMethod)?.description
      } : {
        methodTemplateId: parseInt(selectedMethod)
      }),
      objectives: selectedObjectives.map((objId, index) => {
        // Handle custom items (string IDs starting with 'custom_')
        if (typeof objId === 'string' && objId.startsWith('custom_')) {
          const template = objectiveTemplates.find(t => t.id === objId);
          return {
            objectiveTemplateId: null,
            name: template?.name,
            customContent: template?.content,
            displayOrder: index + 1
          };
        }
        return {
          objectiveTemplateId: objId,
          displayOrder: index + 1
        };
      }),
      skills: selectedSkills.map((skillId, index) => {
        if (typeof skillId === 'string' && skillId.startsWith('custom_')) {
          const template = skillTemplates.find(t => t.id === skillId);
          return {
            skillTemplateId: null,
            name: template?.name,
            description: template?.description,
            displayOrder: index + 1
          };
        }
        return {
          skillTemplateId: skillId,
          displayOrder: index + 1
        };
      }),
      attitudes: selectedAttitudes.map((attId, index) => {
        if (typeof attId === 'string' && attId.startsWith('custom_')) {
          const template = attitudeTemplates.find(t => t.id === attId);
          return {
            attitudeTemplateId: null,
            name: template?.name,
            customContent: template?.content,
            displayOrder: index + 1
          };
        }
        return {
          attitudeTemplateId: attId,
          displayOrder: index + 1
        };
      }),
      languageFocusItems: languageFocus
        .filter(lf => lf.content && lf.content.trim())
        .map((lf, index) => ({
          languageFocusTypeId: lf.typeId ? parseInt(lf.typeId) : null,
          name: lf.name,
          content: lf.content,
          displayOrder: index + 1
        })),
      preparations: selectedPreparations.map((prep, index) => {
        if (typeof prep.id === 'string' && prep.id.startsWith('custom_')) {
          return {
            preparationTypeId: null,
            name: prep.name,
            materials: prep.materials,
            displayOrder: index + 1
          };
        }
        return {
          preparationTypeId: prep.id,
          materials: prep.materials,
          displayOrder: index + 1
        };
      }),
      activityStages: activities.map((stage, stageIndex) => ({
        stageName: stage.stageName,
        displayOrder: stageIndex + 1,
        activityItems: stage.subActivities.map((sub, subIndex) => {
          const activityContent = sub.activityTemplateId
            ? activityTemplates.find(t => t.id.toString() === sub.activityTemplateId.toString())?.content || ''
            : '';

          return {
            timeInMinutes: sub.timeInMinutes || 0,
            content: activityContent,
            // If using template, sub.activityTemplateId is numeric; if custom, it's a string starting with 'custom_'
            ...((typeof sub.interactionPatternId === 'string' && sub.interactionPatternId.startsWith('custom_')) ? {
              interactionPatternId: null,
              interactionPatternName: interactionPatterns.find(p => p.id.toString() === sub.interactionPatternId.toString())?.name,
              interactionPatternShortCode: interactionPatterns.find(p => p.id.toString() === sub.interactionPatternId.toString())?.shortCode,
            } : { interactionPatternId: parseInt(sub.interactionPatternId) }),
            // Same to this
            ...((typeof sub.activityTemplateId === 'string' && sub.activityTemplateId.startsWith('custom_')) ? {
              activityTemplateId: null,
              activityTemplateName: activityTemplates.find(t => t.id.toString() === sub.activityTemplateId.toString())?.name,
              activityTemplateContent: activityContent,
            } : { activityTemplateId: parseInt(sub.activityTemplateId) }),
            displayOrder: subIndex + 1
          };
        })
      }))
    };

    try {
      const token = getAuthToken();
      const result = await putToApi(`/Lesson/planner/${lessonId}`, updateRequest, token);

      if (result.success) {
        setMessage(`✅ Lesson updated successfully! Saving document version...`);

        // Automatically save document version after successful update
        await saveDocumentVersion(token);
      } else {
        throw new Error(result.message || 'An unknown error occurred.');
      }
    } catch (err) {
      console.error('Failed to update lesson:', err);
      setMessage(`❌ Failed to update lesson: ${err.message}`);
    }
  };

  // Save document version to server (called after update)
  const saveDocumentVersion = async (token) => {
    try {
      if (!contentRef.current) return;

      const htmlContent = contentRef.current.innerHTML;
      const title = lessonTitle || 'Lesson_Plan';
      const filename = `${title.replace(/\s/g, '_')}_v${Date.now()}.doc`;
      const html = `<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body><div style="padding: 20px; font-family: Calibri, sans-serif; line-height: 1.6;">${htmlContent}</div></body></html>`;
      const blob = new Blob(['\uFEFF', html], { type: 'application/msword;charset=utf-8' });

      const formData = new FormData();
      formData.append('file', blob, filename);
      formData.append('lessonPlannerId', lessonId);

      const uploadResponse = await fetch(`${API_BASE_URL}/Lesson/planner/upload-document`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formData,
      });

      if (!uploadResponse.ok) {
        console.error('Document save failed with status:', uploadResponse.status);
        setMessage('✅ Lesson updated, but document version save failed.');
        return;
      }

      const uploadResult = await uploadResponse.json();

      if (uploadResult.success) {
        setMessage('✅ Lesson updated and document version saved!');
      } else {
        setMessage('✅ Lesson updated, but document version save failed.');
      }
    } catch (error) {
      console.error('Failed to save document version:', error);
      setMessage('✅ Lesson updated, but document version save failed.');
    }
  };

  // Download and save document to server
  const handleDownloadDocument = async () => {
    if (!contentRef.current) return;

    try {
      const htmlContent = contentRef.current.innerHTML;
      const title = lessonTitle || 'Lesson_Plan';
      const filename = `${title.replace(/\s/g, '_')}.doc`;
      const html = `<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body><div style="padding: 20px; font-family: Calibri, sans-serif; line-height: 1.6;">${htmlContent}</div></body></html>`;
      const blob = new Blob(['\uFEFF', html], { type: 'application/msword;charset=utf-8' });

      // Download to user's computer
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);

      // Upload to server
      const formData = new FormData();
      formData.append('file', blob, filename);
      formData.append('lessonPlannerId', lessonId);

      const token = getAuthToken();
      const uploadResponse = await fetch(`${API_BASE_URL}/Lesson/planner/upload-document`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formData,
      });

      if (!uploadResponse.ok) {
        throw new Error(`Upload failed with status ${uploadResponse.status}`);
      }

      const uploadResult = await uploadResponse.json();

      if (uploadResult.success) {
        setMessage('✅ Lesson plan downloaded and saved to server!');
      } else {
        setMessage('⚠️ Downloaded locally, but server save failed.');
      }
    } catch (error) {
      console.error('Failed to save document to server:', error);
      setMessage('⚠️ Downloaded locally, but server save failed: ' + error.message);
    }
  };

  // generateLessonContent and other UI components are identical to CreateLesson.jsx
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
        const template = objectiveTemplates.find(t => t.id.toString() === objId.toString());
        if (template) {
          html += `<p><strong>${idx + 1}. ${template.name}:</strong> ${template.content}</p>`;
        }
      });
    }
    html += `<h3 style="color: #dc2626; margin-top: 20px;">B. Competences:</h3>`
    const hasLanguageFocus = languageFocus.some(lf => lf.content && lf.content.trim());
    if (hasLanguageFocus) {
      html += `<p><strong>1. Language focus:</strong></p><ul style="list-style-position: inside; padding-left: 10px;">`;
      languageFocus.forEach(lf => {
        if (lf.content && lf.content.trim()) {
          const typeName = lf.typeId ? languageFocusTypes.find(t => t.id.toString() === lf.typeId.toString())?.name : 'Other';
          html += `<li><strong>${typeName || 'Other'}:</strong> ${lf.content}</li>`;
        }
      });
      html += `</ul>`;
    }
    if (selectedSkills.length > 0) {
      html += `<p><strong>2. Skills:</strong></p><ul style="list-style-position: inside; padding-left: 10px;">`;
      selectedSkills.forEach(skillId => {
        const skill = skillTemplates.find(t => t.id.toString() === skillId.toString());
        if (skill) {
          html += `<li><strong>${skill.name}:</strong> ${skill.description}</li>`;
        }
      });
      html += `</ul>`;
    }
    if (selectedAttitudes.length > 0) {
      html += `<p><strong>3. Attitudes:</strong></p><ul style="list-style-position: inside; padding-left: 10px;">`;
      selectedAttitudes.forEach(attId => {
        const att = attitudeTemplates.find(t => t.id.toString() === attId.toString());
        if (att) {
          html += `<li>${att.content}</li>`;
        }
      });
      html += `</ul>`;
    }
    html += `<h3 style="color: #dc2626; margin-top: 20px;">C. Preparations:</h3>`;
    selectedPreparations.forEach(prep => {
      if (prep.materials.trim()) {
        html += `<p>- <strong>${prep.name}:</strong> ${prep.materials}</p>`;
      }
    });
    if (selectedMethod) {
      const method = methodTemplates.find(m => {m.id.toString() === selectedMethod});
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
            const pattern = interactionPatterns.find(p => p.id.toString() === subActivity.interactionPatternId.toString());
            // Handle both regular template IDs and custom activity IDs
            const activityContent = subActivity.activityTemplateId
              ? activityTemplates.find(t => t.id.toString() === subActivity.activityTemplateId.toString())?.content || ''
              : '';

            html += `<tr>`;
            if (subIndex === 0) {
              const totalTime = stage.subActivities.reduce((sum, act) => sum + (Number(act.timeInMinutes) || 0), 0);
              html += `<td style="border: 1px solid #000; padding: 8px; vertical-align: top;" rowspan="${stage.subActivities.length}">
                            <b>${stage.stageName}</b><br/>${totalTime} minutes
                        </td>`;
            }
            html += `<td style="border: 1px solid #000; padding: 8px; vertical-align: top;">${activityContent.replace(/\n/g, '<br/>')}</td>`;
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

  const RichTextToolbar = () => (<div className="flex flex-wrap items-center p-3 border-b bg-gray-100 rounded-t-xl sticky top-0 z-10"> <button className="ml-auto flex items-center space-x-2 px-3 py-1.5 text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition shadow" onClick={handleUpdateLesson}> <Save className="w-4 h-4" /> <span className="text-sm font-medium">Update Lesson</span> </button> </div>);
  const StepIndicator = () => (<div className="flex items-center justify-center mb-8"> {[1, 2, 3, 4, 5, 6].map((step) => (<React.Fragment key={step}> <div className={`flex flex-col items-center cursor-pointer ${step <= currentStep ? 'text-blue-600' : 'text-gray-400'}`} onClick={() => setCurrentStep(step)}> <div className={`w-10 h-10 rounded-full flex items-center justify-center font-bold transition-all duration-300 ${step < currentStep ? 'bg-green-500 text-white' : step === currentStep ? 'bg-blue-600 text-white scale-110' : 'bg-gray-300'}`}> {step < currentStep ? <CheckCircle2 className="w-6 h-6" /> : step} </div> <span className="text-xs mt-1 hidden sm:block"> {['Basic', 'Objectives', 'Skills', 'Attitudes', 'Prep', 'Activities'][step - 1]} </span> </div> {step < 6 && <div className={`h-1 w-12 sm:w-20 transition-all duration-500 ${step < currentStep ? 'bg-green-500' : 'bg-gray-300'}`} />} </React.Fragment>))} </div>);
  const ItemSelector = ({ title, templates, selectedIds, onAdd, onRemove, selectedValue, onSelectChange }) => {
    const availableItems = templates.filter(t => !selectedIds.includes(t.id));
    return (
      <div className="space-y-4">
        <div className="flex items-center gap-2">
          <select value={selectedValue} onChange={onSelectChange} className="w-full p-2 border border-gray-300 rounded-lg text-sm" disabled={availableItems.length === 0} >
            <option value="">{availableItems.length > 0 ? `Select a ${title.toLowerCase()}...` : `All ${title.toLowerCase()} added`}</option>
            {availableItems.map(item => (
              <option key={item.id} value={item.id}>
                {item.isCustom ? '✏️ ' : ''}{item.name}
              </option>
            ))}
          </select>
          <button onClick={() => onAdd(selectedValue)} disabled={!selectedValue} className="px-3 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:bg-gray-300 disabled:cursor-not-allowed" >
            <Plus className="w-5 h-5" />
          </button>
        </div>
        <div className="space-y-2">
          {selectedIds.length > 0 ? (
            selectedIds.map(id => {
              const item = templates.find(t => t.id.toString() === id.toString());
              if (!item) return null;
              return (
                <div key={id} className={`flex items-center justify-between p-2 border rounded-lg animate-fade-in ${item.isCustom ? 'bg-amber-50 border-amber-200' : 'bg-gray-100'}`}>
                  <div className="text-sm flex-1">
                    <p className="font-semibold flex items-center gap-1">
                      {item.isCustom && <span className="text-amber-600 text-xs">✏️ Custom</span>}
                      {item.name}
                    </p>
                    <p className="text-gray-600">{item.content || item.description}</p>
                  </div>
                  <button onClick={() => onRemove(id)} className="text-red-500 hover:text-red-700 p-1">
                    <X className="w-4 h-4" />
                  </button>
                </div>
              );
            })
          ) : (
            <p className="text-sm text-gray-500 text-center py-2">No {title.toLowerCase()} added yet.</p>
          )}
        </div>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 p-8 flex flex-col items-center justify-center text-center">
        <Loader2 className="w-12 h-12 text-blue-600 animate-spin mb-4" />
        <h2 className="text-xl font-semibold text-gray-700">Loading Lesson Plan...</h2>
        <p className="text-gray-500">Please wait while we fetch the details.</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 p-8 flex flex-col items-center justify-center text-center">
        <AlertTriangle className="w-12 h-12 text-red-500 mb-4" />
        <h2 className="text-xl font-semibold text-red-700">An Error Occurred</h2>
        <p className="text-gray-600 max-w-md my-2">{error}</p>
        <button
          onClick={() => navigate('/teacher/lesson-planner')} // Adjust this route as needed
          className="mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
        >
          Back to Lesson Plans
        </button>
      </div>
    )
  }

  // =================================================================
  // JSX RENDER (Identical structure to CreateLesson)
  // =================================================================
  return (
    <div className="min-h-screen bg-gray-50 p-4 sm:p-8 font-['Inter']">
      <header className="mb-8">
        <h1 className="text-3xl font-extrabold text-gray-900 flex items-center">
          <BookOpen className="w-8 h-8 mr-3 text-red-600" />
          Edit Lesson Plan
        </h1>
        <p className="text-gray-500 mt-1">Modify the details of your lesson plan (ID: {lessonId}).</p>
      </header>

      {message && (
        <div className={`mb-4 p-3 rounded-lg shadow-sm text-sm font-medium animate-fade-in ${message.startsWith('✅') ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
          {message}
        </div>
      )}

      <StepIndicator />

      {/* The rest of the JSX is identical to CreateLesson.jsx, so it's omitted for brevity but should be copied here. */}
      {/* Just ensure button text and handlers are updated, e.g., "Update Lesson" instead of "Save Lesson". */}
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
                    <select value={selectedGradeId} onChange={(e) => {
                      setSelectedGradeId(e.target.value);
                      setSelectedClassId(''); // Reset class when grade changes
                    }} className="w-full p-3 border border-gray-300 rounded-lg">
                      <option value="">Select Grade</option>
                      {gradeLevels.map(grade => <option key={grade.id} value={grade.id}>{grade.name}</option>)}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-bold text-gray-700 mb-2">Class</label>
                    <select value={selectedClassId} onChange={(e) => setSelectedClassId(e.target.value)} disabled={!selectedGradeId} className="w-full p-3 border border-gray-300 rounded-lg">
                      <option value="">{!selectedGradeId ? 'Select grade first' : 'Select Class'}</option>
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
              {/* ... Other steps (3, 4, 5, 6) are identical to CreateLesson.jsx */}
              {currentStep === 3 && (<div className="space-y-4"> <div className="flex items-center space-x-2 text-blue-600 mb-4"> <Brain className="w-6 h-6" /> <h2 className="text-xl font-bold">Skills Development</h2> </div> <ItemSelector title="Skill" templates={skillTemplates} selectedIds={selectedSkills} onAdd={(id) => handleAddItem(id, selectedSkills, setSelectedSkills, setSkillToAdd)} onRemove={(id) => handleRemoveItem(id, selectedSkills, setSelectedSkills)} selectedValue={skillToAdd} onSelectChange={(e) => setSkillToAdd(e.target.value)} /> </div>)}
              {currentStep === 4 && (<div className="space-y-6"> <div className="flex items-center space-x-2 text-blue-600 mb-2"> <Heart className="w-6 h-6" /> <h2 className="text-xl font-bold">Attitudes & Language</h2> </div> <div> <h3 className="font-bold text-gray-700 mb-3">Attitudes</h3> <ItemSelector title="Attitude" templates={attitudeTemplates} selectedIds={selectedAttitudes} onAdd={(id) => handleAddItem(id, selectedAttitudes, setSelectedAttitudes, setAttitudeToAdd)} onRemove={(id) => handleRemoveItem(id, selectedAttitudes, setSelectedAttitudes)} selectedValue={attitudeToAdd} onSelectChange={(e) => setAttitudeToAdd(e.target.value)} /> </div> <div className="mt-6"> <h3 className="font-bold text-gray-700 mb-3">Language Focus</h3> <div className="space-y-3"> {languageFocus.map((lf, idx) => (<div key={idx} className="flex gap-2 items-start"> <div className="flex-1"> <select value={lf.typeId || ''} onChange={(e) => { const updated = [...languageFocus]; updated[idx].typeId = e.target.value; setLanguageFocus(updated); }} className="w-full p-2 border border-gray-300 rounded-lg text-sm mb-1" > <option value="">Select type...</option> {languageFocusTypes.map(type => (<option key={type.id} value={type.id}>{type.name}</option>))} </select> <textarea rows="2" placeholder="Enter content..." value={lf.content} onChange={(e) => { const updated = [...languageFocus]; updated[idx].content = e.target.value; setLanguageFocus(updated); }} className="w-full p-2 border border-gray-300 rounded-lg text-sm" /> </div> <button onClick={() => { const updated = languageFocus.filter((_, i) => i !== idx); setLanguageFocus(updated.length > 0 ? updated : [{ typeId: null, content: '' }]); }} className="text-red-500 hover:text-red-700 p-1 mt-1" > <X className="w-4 h-4" /> </button> </div>))} <button onClick={() => setLanguageFocus([...languageFocus, { typeId: null, content: '' }])} className="w-full py-2 px-4 bg-green-100 text-green-800 rounded-lg hover:bg-green-200 flex items-center justify-center space-x-2" > <Plus className="w-4 h-4" /> <span>Add Language Focus Item</span> </button> </div> </div> </div>)}
              {currentStep === 5 && (<div className="space-y-6"> <div className="flex items-center space-x-2 text-blue-600 mb-2"> <Book className="w-6 h-6" /> <h2 className="text-xl font-bold">Preparations & Methods</h2> </div> <div> <h3 className="font-bold text-gray-700 mb-3">Preparations</h3> <div className="space-y-4"> <div className="flex items-center gap-2"> <select value={preparationToAdd} onChange={(e) => setPreparationToAdd(e.target.value)} className="w-full p-2 border border-gray-300 rounded-lg text-sm" > <option value="">Select a preparation type...</option> {preparationTemplates.filter(t => !selectedPreparations.some(p => p.id.toString() === t.id.toString())).map(template => (<option key={template.id} value={template.id}>{template.name}</option>))} </select> <button onClick={() => handleAddPreparation(preparationToAdd)} disabled={!preparationToAdd} className="px-3 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:bg-gray-300" > <Plus className="w-5 h-5" /> </button> </div> <div className="space-y-3"> {selectedPreparations.map((prep) => (<div key={prep.id} className="p-3 bg-gray-50 border rounded-lg"> <div className="flex justify-between items-center mb-1"> <label className="block text-sm font-semibold text-gray-700">{prep.name}</label> <button onClick={() => handleRemovePreparation(prep.id)} className="text-red-500 hover:text-red-700 p-1"> <X className="w-4 h-4" /> </button> </div> <p className="text-sm text-gray-800 p-2 bg-white border rounded-md whitespace-pre-wrap"> {prep.materials} </p> </div>))} </div> </div> </div> <div className="mt-6"> <h3 className="font-bold text-gray-700 mb-3">Teaching Method</h3> <select value={selectedMethod} onChange={(e) => setSelectedMethod(e.target.value)} className="w-full p-3 border border-gray-300 rounded-lg"> <option value="">Select a teaching method</option> {methodTemplates.map(method => (<option key={method.id} value={method.id}>{method.name}</option>))} </select> {selectedMethod && (<p className="text-sm text-gray-600 mt-2 p-2 bg-gray-50 rounded-md"> {methodTemplates.find(m => m.id.toString() === selectedMethod)?.description} </p>)} </div> </div>)}
            </>
          )}

          {currentStep === 6 && (<div className="space-y-4"> <div className="flex items-center space-x-2 text-blue-600 mb-4"> <Users className="w-6 h-6" /> <h2 className="text-xl font-bold">Lesson Activities</h2> </div> <div className="space-y-6 max-h-[60vh] overflow-y-auto pr-2"> {activities.map((stage, stageIndex) => (<div key={stageIndex} className="p-4 border-2 border-gray-200 rounded-lg bg-gray-50 space-y-4"> <div className="flex justify-between items-center pb-2 border-b"> <input type="text" value={stage.stageName} onChange={(e) => updateStageName(stageIndex, e.target.value)} className="font-bold text-lg text-gray-800 p-1 rounded bg-transparent focus:bg-white" /> <button onClick={() => removeStage(stageIndex)} className="text-red-500 hover:text-red-700 p-1 rounded-full hover:bg-red-100"> <X className="w-4 h-4" /> </button> </div> {stage.subActivities.map((sub, subIndex) => (<div key={subIndex} className="p-3 border rounded-md bg-white relative"> <span className="absolute -left-2 top-2 text-xs bg-blue-500 text-white font-bold rounded-full h-5 w-5 flex items-center justify-center">{subIndex + 1}</span> <div className="space-y-2 pl-4"> <div> <label className="block text-xs font-semibold text-gray-600 mb-1">Time (minutes)</label> <input type="number" min="0" value={sub.timeInMinutes} onChange={(e) => updateSubActivity(stageIndex, subIndex, 'timeInMinutes', parseInt(e.target.value) || 0)} className="w-full p-2 border border-gray-300 rounded text-sm" /> </div> <div> <label className="block text-xs font-semibold text-gray-600 mb-1">Activity Template</label> <select value={sub.activityTemplateId} onChange={(e) => updateSubActivity(stageIndex, subIndex, 'activityTemplateId', e.target.value)} className="w-full p-2 border border-gray-300 rounded text-sm" > <option value="">-- Select an activity template --</option> {activityTemplates.map(template => (<option key={template.id} value={template.id}>{template.name}</option>))} </select> {sub.activityTemplateId && activityTemplates.find(t => t.id.toString() === sub.activityTemplateId.toString()) && (<div className="mt-2 p-2 bg-blue-50 border border-blue-200 rounded text-xs"> <strong>Template Content:</strong> <p className="mt-1 whitespace-pre-wrap break-words line-clamp-3 overflow-hidden">{activityTemplates.find(t => t.id.toString() === sub.activityTemplateId.toString())?.content}</p> </div>)} </div> <div> <label className="block text-xs font-semibold text-gray-600 mb-1">Interaction Pattern</label> <select value={sub.interactionPatternId} onChange={(e) => updateSubActivity(stageIndex, subIndex, 'interactionPatternId', e.target.value)} className="w-full p-2 border border-gray-300 rounded text-sm"> <option value="">Select interaction</option> {interactionPatterns.map(pattern => (<option key={pattern.id} value={pattern.id}>{pattern.name} ({pattern.shortCode})</option>))} </select> </div> </div> {stage.subActivities.length > 0 && (<button onClick={() => removeSubActivity(stageIndex, subIndex)} className="absolute top-1 right-1 text-gray-400 hover:text-red-600"> <X className="w-3 h-3" /> </button>)} </div>))} <button onClick={() => addSubActivity(stageIndex)} className="w-full text-xs py-1 px-2 bg-blue-100 text-blue-800 rounded hover:bg-blue-200 flex items-center justify-center space-x-1"> <Plus className="w-3 h-3" /> <span>Add Activity to this Stage</span> </button> </div>))} </div> <button onClick={addStage} className="w-full py-2 px-4 bg-green-600 text-white rounded-lg hover:bg-green-700 flex items-center justify-center space-x-2"> <Plus className="w-4 h-4" /> <span>Add New Stage</span> </button> </div>)}
          <div className="flex justify-between mt-6 pt-4 border-t">
            <button onClick={handlePrevStep} disabled={currentStep === 1} className={`flex items-center space-x-2 px-4 py-2 rounded-lg transition ${currentStep === 1 ? 'bg-gray-300 text-gray-500 cursor-not-allowed' : 'bg-gray-600 text-white hover:bg-gray-700'}`}>
              <ChevronLeft className="w-4 h-4" />
              <span>Previous</span>
            </button>
            <button onClick={handleNextStep} className="flex items-center space-x-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 shadow-md">
              <span>{currentStep === totalSteps ? 'Generate Preview' : 'Next'}</span>
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
                <p className="text-lg">Click "Generate Preview" to see your lesson plan</p>
                <p className="text-sm mt-2">The preview will appear here once generated.</p>
              </div>
            </div>
          </div>

          <div className="mt-6 flex justify-end space-x-4 flex-wrap gap-2">
            <button onClick={() => { if (contentRef.current) { navigator.clipboard.writeText(contentRef.current.innerText); setMessage('Lesson content (plain text) copied to clipboard!'); } }} className="flex items-center space-x-2 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition shadow-md">
              <Copy className="w-4 h-4" />
              <span>Copy text</span>
            </button>
            <button onClick={handleDownloadDocument} className="flex items-center space-x-2 px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300 transition shadow-md">
              <Download className="w-4 h-4" />
              <span>Download .doc</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
