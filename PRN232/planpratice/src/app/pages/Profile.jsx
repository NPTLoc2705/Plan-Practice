// src/routes/Dashboard.jsx
import { useNavigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';

const Profile = () => {
  const navigate = useNavigate();
  const user = AuthAPI.getUser();

  const handleLogout = () => {
    AuthAPI.logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <h1 className="text-xl font-bold text-gray-800">IELTS Learning Platform</h1>
            <div className="flex items-center gap-4">
              <div className="text-sm text-gray-600">
                Welcome, <span className="font-medium">{user?.username || 'User'}</span>
              </div>
              <button
                onClick={handleLogout}
                className="px-4 py-2 text-sm bg-red-500 hover:bg-red-600 text-white rounded-lg transition"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>
      
      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-2xl font-bold mb-4">Welcome to your Dashboard</h2>
          <div className="grid gap-4">
            <div className="p-4 bg-blue-50 rounded-lg">
              <h3 className="font-semibold text-blue-900 mb-2">User Information</h3>
              <div className="space-y-1 text-sm text-blue-700">
                <p><span className="font-medium">ID:</span> {user?.id}</p>
                <p><span className="font-medium">Username:</span> {user?.username}</p>
                <p><span className="font-medium">Email:</span> {user?.email}</p>
                <p><span className="font-medium">Role:</span> {user?.role}</p>
                <p><span className="font-medium">Created:</span> {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : 'N/A'}</p>
              </div>
            </div>
            
            <div className="p-4 bg-green-50 rounded-lg">
              <h3 className="font-semibold text-green-900 mb-2">Quick Actions</h3>
              <div className="flex flex-wrap gap-2">
                <button className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg text-sm transition">
                  Create Study Plan
                </button>
                <button className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm transition">
                  Browse Lessons
                </button>
                <button className="px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg text-sm transition">
                  Take Quiz
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile;