// src/pages/AdminDashboard.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminAPI from '../components/APIService/AdminAPI';

// Import sub-components
import DashboardOverview from '../components/Admin/DashboardOverview';
import UserManagement from '../components/Admin/UserManagement';
import RevenueManagement from '../components/Admin/RevenueManagement';
import PackageManagement from '../components/Admin/PackageManagement';

// Icons (you can use react-icons or heroicons)
import { 
  HomeIcon, 
  UsersIcon, 
  CurrencyDollarIcon, 
  CubeIcon, 
  DocumentTextIcon,
  ArrowRightOnRectangleIcon,
  UserCircleIcon,
  Cog6ToothIcon
} from '@heroicons/react/24/outline';

const AdminDashboard = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('overview');
  const [loading, setLoading] = useState(false);
  const [adminInfo, setAdminInfo] = useState({
    name: 'Admin User',
    email: 'admin@example.com',
    role: 'Administrator'
  });
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalStudents: 0,
    totalTeachers: 0,
    todayRevenue: 0,
    monthRevenue: 0,
    yearRevenue: 0
  });
  const [showLogoutConfirm, setShowLogoutConfirm] = useState(false);

  useEffect(() => {
    fetchDashboardStats();
    // Get admin info from localStorage or session
    const storedAdminInfo = localStorage.getItem('adminInfo');
    if (storedAdminInfo) {
      setAdminInfo(JSON.parse(storedAdminInfo));
    }
  }, []);

  const fetchDashboardStats = async () => {
    setLoading(true);
    try {
      const [users, students, teachers, todayRev, monthRev, yearRev] = await Promise.all([
        AdminAPI.getUsers(),
        AdminAPI.getStudents(),
        AdminAPI.getTeachers(),
        AdminAPI.getTodayRevenue(),
        AdminAPI.getMonthRevenue(),
        AdminAPI.getYearRevenue()
      ]);

      setStats({
        totalUsers: users.data?.length || 0,
        totalStudents: students.data?.length || 0,
        totalTeachers: teachers.data?.length || 0,
        todayRevenue: todayRev.data?.total || 0,
        monthRevenue: monthRev.data?.total || 0,
        yearRevenue: yearRev.data?.total || 0
      });
    } catch (error) {
      console.error('Failed to fetch dashboard stats:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    // Clear authentication tokens and user data
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    sessionStorage.clear();
    
    // Redirect to login page
    navigate('/login');
    
    // Optional: Call logout API if you have one
    // AdminAPI.logout();
  };

  const renderContent = () => {
    switch(activeTab) {
      case 'overview':
        return <DashboardOverview stats={stats} loading={loading} />;
      case 'users':
        return <UserManagement />;
      case 'revenue':
        return <RevenueManagement />;
      case 'packages':
        return <PackageManagement />;
      default:
        return <DashboardOverview stats={stats} loading={loading} />;
    }
  };

  const navItems = [
    { id: 'overview', label: 'Overview', icon: HomeIcon },
    { id: 'users', label: 'Users', icon: UsersIcon },
    { id: 'revenue', label: 'Revenue', icon: CurrencyDollarIcon },
    { id: 'packages', label: 'Packages', icon: CubeIcon },
  ];

  return (
    <div className="flex h-screen bg-gray-100">
      {/* Sidebar */}
      <div className="w-64 bg-gray-900 text-white shadow-lg flex flex-col">
        {/* Header */}
        <div className="p-6 border-b border-gray-800">
          <h2 className="text-2xl font-bold mb-1">Admin Panel</h2>
          <p className="text-xs text-gray-400">Management Dashboard</p>
        </div>

        {/* Admin Profile Section */}
        <div className="px-6 py-4 border-b border-gray-800">
          <div className="flex items-center space-x-3">
            <div className="flex-shrink-0">
              <div className="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
                <UserCircleIcon className="w-6 h-6 text-white" />
              </div>
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium truncate">{adminInfo.name}</p>
              <p className="text-xs text-gray-400 truncate">{adminInfo.role}</p>
            </div>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 mt-6">
          {navItems.map((item) => {
            const Icon = item.icon;
            return (
              <button
                key={item.id}
                className={`w-full flex items-center px-6 py-3 text-left transition-colors duration-200 hover:bg-gray-800 ${
                  activeTab === item.id 
                    ? 'bg-blue-600 border-l-4 border-blue-400' 
                    : ''
                }`}
                onClick={() => setActiveTab(item.id)}
              >
                <Icon className="w-5 h-5 mr-3" />
                <span className="text-sm font-medium">{item.label}</span>
              </button>
            );
          })}
        </nav>

        {/* Bottom Section with Settings and Logout */}
        <div className="border-t border-gray-800">
         
          
          <button
            className="w-full flex items-center px-6 py-3 text-left transition-colors duration-200 hover:bg-red-600 group"
            onClick={() => setShowLogoutConfirm(true)}
          >
            <ArrowRightOnRectangleIcon className="w-5 h-5 mr-3 group-hover:animate-pulse" />
            <span className="text-sm font-medium">Logout</span>
          </button>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 overflow-y-auto">
        <div className="p-8">
          {renderContent()}
        </div>
      </div>

      {/* Logout Confirmation Modal */}
      {showLogoutConfirm && (
        <div className="fixed inset-0 z-50 overflow-y-auto">
          <div className="flex items-center justify-center min-h-screen px-4">
            <div 
              className="fixed inset-0 bg-black opacity-50" 
              onClick={() => setShowLogoutConfirm(false)}
            ></div>
            
            <div className="relative bg-white rounded-2xl max-w-md w-full p-6 shadow-2xl">
              <div className="flex items-center gap-3 mb-4">
                <div className="h-12 w-12 rounded-full bg-red-100 flex items-center justify-center">
                  <ArrowRightOnRectangleIcon className="h-6 w-6 text-red-600" />
                </div>
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">Confirm Logout</h3>
                  <p className="text-sm text-gray-600">Are you sure you want to logout?</p>
                </div>
              </div>

              <p className="text-gray-700 mb-6">
                You will be redirected to the login page and will need to sign in again to access the admin dashboard.
              </p>
              
              <div className="flex justify-end gap-3">
                <button
                  onClick={() => setShowLogoutConfirm(false)}
                  className="px-5 py-2.5 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  onClick={handleLogout}
                  className="px-5 py-2.5 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
                >
                  Logout
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminDashboard;