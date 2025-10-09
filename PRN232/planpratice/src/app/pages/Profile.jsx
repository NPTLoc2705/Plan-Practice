import { useNavigate } from 'react-router-dom';
import { AuthAPI } from '../components/APIService/AuthAPI';

const Profile = () => {
  const navigate = useNavigate();
  const user = AuthAPI.getUser();

 

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-cyan-50 to-white flex flex-col">
      <main className="flex-1 flex items-center justify-center px-4">
        <div className="w-full max-w-2xl bg-white rounded-2xl shadow-xl p-8">
          <div className="mb-8">
            <h2 className="text-xl font-bold text-gray-900 mb-4">Your Info</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            
              <div className="bg-blue-50 rounded-lg p-4">
                <span className="font-semibold text-gray-700">Username:</span> {user?.username}
              </div>
              <div className="bg-blue-50 rounded-lg p-4">
                <span className="font-semibold text-gray-700">Email:</span> {user?.email}
              </div>
              
              <div className="bg-blue-50 rounded-lg p-4 md:col-span-2">
                <span className="font-semibold text-gray-700">Created:</span> {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : 'N/A'}
              </div>
            </div>
          </div>

          <div>
            <h3 className="text-lg font-bold text-gray-900 mb-4">Quick Actions</h3>
            <div className="flex flex-wrap gap-4">
              <button className="px-6 py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700 transition">
                Create Study Plan
              </button>
              <button className="px-6 py-3 bg-cyan-600 text-white rounded-lg font-semibold hover:bg-cyan-700 transition">
                Browse Lessons
              </button>
              <button className="px-6 py-3 bg-purple-600 text-white rounded-lg font-semibold hover:bg-purple-700 transition">
                Take Quiz
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Profile;