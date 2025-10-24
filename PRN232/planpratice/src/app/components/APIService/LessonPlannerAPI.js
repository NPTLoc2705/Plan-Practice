// src/components/APIService/LessonPlannerAPI.js
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const headers = () => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${localStorage.getItem('token')}`,
});

export class LessonPlannerAPI {
    // Get lesson planners for the current user
    static async getMyLessonPlanners() {
        try {
            const response = await fetch(`${API_BASE_URL}/lessonplanner/my-planners`, {
                method: 'GET',
                headers: headers(),
            });

            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || 'Failed to fetch lesson planners');
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Error in getMyLessonPlanners:', error);
            throw new Error('Failed to fetch lesson planners');
        }
    }

    // Get lesson planner by ID
    static async getLessonPlannerById(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/lessonplanner/${id}`, {
                method: 'GET',
                headers: headers(),
            });

            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || 'Failed to fetch lesson planner');
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Error in getLessonPlannerById:', error);
            throw new Error('Failed to fetch lesson planner');
        }
    }
}

export default LessonPlannerAPI;