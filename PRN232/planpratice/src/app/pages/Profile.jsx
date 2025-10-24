import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { AuthAPI } from '../components/APIService/AuthAPI';

const Profile = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(AuthAPI.getUser());
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    userName: user?.username || '',
    phone: user?.phone || '',
    oldPassword: '',
    newPassword: ''
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const handleEditToggle = () => {
    setIsEditing((prev) => !prev);
    // Reset form data when canceling
    if (isEditing) {
      setFormData({
        userName: user?.username || '',
        phone: user?.phone || '',
        oldPassword: '',
        newPassword: ''
      });
      setMessage('');
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSave = async () => {
    try {
      setLoading(true);
      setMessage('');
      
      // Prepare update data (only include fields that have values)
      const updateData = {};
       if (formData.userName !== user.username) {
      updateData.userName = formData.userName || ''; // Allow empty to clear
    }
      if (formData.phone !== user.phone) {
      updateData.phone = formData.phone || ''; // Allow empty to clear phone
    }
     if (formData.newPassword) {
      updateData.oldPassword = formData.oldPassword;
      updateData.newPassword = formData.newPassword;
    }

      // If no changes, just exit edit mode
      if (Object.keys(updateData).length === 0) {
        setIsEditing(false);
        return;
      }

      const result = await AuthAPI.updateProfile(updateData);
      
      setMessage(result.message || 'Profile updated successfully');
      
      // Update local user state
      const updatedUser = AuthAPI.getUser();
      setUser(updatedUser);
      
      setIsEditing(false);
      
      // Clear password fields
      setFormData(prev => ({
        ...prev,
        oldPassword: '',
        newPassword: ''
      }));
      
    } catch (error) {
      setMessage(error.message || 'Failed to update profile');
    } finally {
      setLoading(false);
    }
  };

  // Format date properly
  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    } catch {
      return 'Invalid Date';
    }
  };

  // Convert numeric role to display text
  const getRoleDisplay = (role) => {
    if (role === undefined || role === null) return 'student';
    
    switch (role) {
      case 0:
        return 'student';
      case 1:
        return 'teacher';
      case 2:
        return 'admin';
      default:
        return 'student';
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-cyan-50 to-white flex flex-col">
      <main className="flex-1 flex items-center justify-center px-4 py-12">
        <div className="w-full max-w-2xl bg-white rounded-2xl shadow-2xl p-8 relative overflow-hidden">
          {/* Decorative gradient ring */}
          <div className="absolute inset-0 opacity-10 bg-gradient-to-tr from-blue-400 to-cyan-400 blur-3xl"></div>

          <div className="relative z-10">
            {/* Header */}
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-2xl font-extrabold text-gray-900">Profile</h2>
              <button
                onClick={handleEditToggle}
                disabled={loading}
                className="px-4 py-2 rounded-lg font-semibold text-sm bg-blue-600 text-white hover:bg-blue-700 transition disabled:opacity-50"
              >
                {isEditing ? 'Cancel' : 'Edit Profile'}
              </button>
            </div>

            {/* Message Display */}
            {message && (
              <div className={`mb-4 p-3 rounded-lg ${
                message.includes('successfully') 
                  ? 'bg-green-100 text-green-800' 
                  : 'bg-red-100 text-red-800'
              }`}>
                {message}
              </div>
            )}

            {/* User Info Section */}
            <div className="mb-8">
              <h3 className="text-lg font-bold text-gray-800 mb-4">Your Info</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="bg-blue-50 rounded-lg p-4">
                  <span className="font-semibold text-gray-700">Username:</span>{' '}
                  {isEditing ? (
                    <input
                      type="text"
                      name="userName"
                      value={formData.userName}
                      onChange={handleChange}
                      className="ml-2 border border-gray-300 rounded-md px-2 py-1 text-sm focus:outline-none focus:ring focus:ring-blue-200"
                    />
                  ) : (
                    <span className="ml-1 text-gray-900">{user?.username || 'N/A'}</span>
                  )}
                </div>

                <div className="bg-blue-50 rounded-lg p-4">
                  <span className="font-semibold text-gray-700">Email:</span>{' '}
                  <span className="ml-1 text-gray-900">{user?.email || 'N/A'}</span>
                  <span className="ml-2 text-sm text-green-600">
                    {/* {user?.emailVerified ? '✓ Verified' : '✗ Not Verified'} */}
                  </span>
                </div>

                <div className="bg-blue-50 rounded-lg p-4">
                  <span className="font-semibold text-gray-700">Phone:</span>{' '}
                  {isEditing ? (
                    <input
                      type="text"
                      name="phone"
                      value={formData.phone}
                      onChange={handleChange}
                      placeholder="Add phone number"
                      className="ml-2 border border-gray-300 rounded-md px-2 py-1 text-sm focus:outline-none focus:ring focus:ring-blue-200"
                    />
                  ) : (
                    <span className="ml-1 text-gray-900">
                      {user?.phone || 'Not provided'}
                    </span>
                  )}
                </div>

                <div className="bg-blue-50 rounded-lg p-4">
                  <span className="font-semibold text-gray-700">Role:</span>{' '}
                  <span className="ml-1 text-gray-900 capitalize">
                    {getRoleDisplay(user?.role)}
                  </span>
                </div>

                <div className="bg-blue-50 rounded-lg p-4 md:col-span-2">
                  <span className="font-semibold text-gray-700">Member Since:</span>{' '}
                  <span className="ml-1 text-gray-900">
                    {formatDate(user?.createdAt)}
                  </span>
                </div>

                {/* Password Change Fields */}
                {isEditing && (
                  <>
                    <div className="bg-yellow-50 rounded-lg p-4 md:col-span-2">
                      <h4 className="font-semibold text-gray-700 mb-2">Change Password</h4>
                      <div className="space-y-2">
                        <input
                          type="password"
                          name="oldPassword"
                          value={formData.oldPassword}
                          onChange={handleChange}
                          placeholder="Current password"
                          className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring focus:ring-blue-200"
                        />
                        <input
                          type="password"
                          name="newPassword"
                          value={formData.newPassword}
                          onChange={handleChange}
                          placeholder="New password (leave empty to keep current)"
                          className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring focus:ring-blue-200"
                        />
                      </div>
                    </div>
                  </>
                )}
              </div>

              {isEditing && (
                <div className="mt-6 flex justify-end">
                  <button
                    onClick={handleSave}
                    disabled={loading}
                    className="px-6 py-2 bg-green-600 text-white font-semibold rounded-lg hover:bg-green-700 transition disabled:opacity-50"
                  >
                    {loading ? 'Saving...' : 'Save Changes'}
                  </button>
                </div>
              )}
            </div>

            {/* Quick Actions Section */}
            <div>
              {/* <h3 className="text-lg font-bold text-gray-800 mb-4">
                Quick Actions
              </h3> */}
              <div className="flex flex-wrap gap-4">
                {/* <button
                  onClick={() => navigate('/studyplan')}
                  className="px-6 py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700 transition"
                >
                  Create Study Plan
                </button>
                <button
                  onClick={() => navigate('/lessons')}
                  className="px-6 py-3 bg-cyan-600 text-white rounded-lg font-semibold hover:bg-cyan-700 transition"
                >
                  Browse Lessons
                </button>
                <button
                  onClick={() => navigate('/quiz')}
                  className="px-6 py-3 bg-purple-600 text-white rounded-lg font-semibold hover:bg-purple-700 transition"
                >
                  Take Quiz
                </button> */}
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Profile;