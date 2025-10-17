import React, { useState, useEffect } from 'react';
import { Search, RefreshCw, GraduationCap, BookOpen, Users, Shield, BarChart3, Filter, Ban, UserCheck } from 'lucide-react';

// Components
import Toast from '../components/Toast/Toast';
import Pagination from '../components/Pagination/Pagination';
import UserTable from '../components/UserTable/UserTable';

// Services & Hooks
import AdminAPI from '../components/APIService/AdminAPI';
import { useToast } from '../components/Toast/useToast';

const AdminDashboard = () => {
  const [students, setStudents] = useState([]);
  const [teachers, setTeachers] = useState([]);
  const [loading, setLoading] = useState({ students: false, teachers: false });
  const [searchTerm, setSearchTerm] = useState('');
  const [banFilter, setBanFilter] = useState('all'); // 'all', 'banned', 'active'
  const [currentPage, setCurrentPage] = useState({ students: 1, teachers: 1 });
  const [editingUser, setEditingUser] = useState(null);
  const [activeTab, setActiveTab] = useState('overview');
  const [stats, setStats] = useState({
    totalStudents: 0,
    totalTeachers: 0,
    bannedUsers: 0,
    activeUsers: 0
  });
  
  const { toast, showToast, hideToast } = useToast();

  const ITEMS_PER_PAGE = 10;

  useEffect(() => {
    fetchStudents();
    fetchTeachers();
  }, []);

  useEffect(() => {
    // Update stats when data changes
    const bannedStudents = students.filter(s => s.isBanned).length;
    const bannedTeachers = teachers.filter(t => t.isBanned).length;
    
    setStats({
      totalStudents: students.length,
      totalTeachers: teachers.length,
      bannedUsers: bannedStudents + bannedTeachers,
      activeUsers: (students.length + teachers.length) - (bannedStudents + bannedTeachers)
    });
  }, [students, teachers]);

  const fetchStudents = async () => {
    setLoading(prev => ({ ...prev, students: true }));
    try {
      const response = await AdminAPI.getStudents();
      setStudents(response.data);
    } catch (error) {
      showToast('Failed to load students', 'error');
    } finally {
      setLoading(prev => ({ ...prev, students: false }));
    }
  };

  const fetchTeachers = async () => {
    setLoading(prev => ({ ...prev, teachers: true }));
    try {
      const response = await AdminAPI.getTeachers();
      setTeachers(response.data);
    } catch (error) {
      showToast('Failed to load teachers', 'error');
    } finally {
      setLoading(prev => ({ ...prev, teachers: false }));
    }
  };

  const handleRefreshAll = () => {
    fetchStudents();
    fetchTeachers();
    showToast('Refreshing all data...', 'info');
  };

  const handleBanToggle = async (user) => {
    try {
      if (user.isBanned) {
        await AdminAPI.unbanUser(user.id);
      } else {
        await AdminAPI.banUser(user.id);
      }

      const updateList = user.role === 'Student' ? setStudents : setTeachers;
      updateList(prev => prev.map(u => 
        u.id === user.id ? { ...u, isBanned: !u.isBanned } : u
      ));

      showToast(
        `User ${user.isBanned ? 'unbanned' : 'banned'} successfully`,
        'success'
      );
    } catch (error) {
      showToast('Failed to update user status', 'error');
    }
  };

  const handleEditUser = async (userId, data) => {
    try {
      await AdminAPI.updateUser(userId, data);

      const updateList = editingUser.role === 'Student' ? setStudents : setTeachers;
      updateList(prev => prev.map(u => 
        u.id === userId ? { ...u, ...data } : u
      ));

      showToast('User updated successfully', 'success');
      setEditingUser(null);
    } catch (error) {
      showToast('Failed to update user', 'error');
    }
  };

  // Enhanced filter function with ban status
  const filterUsers = (users) => {
    let filtered = users;

    // Apply search filter
    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      filtered = filtered.filter(user => 
        user.username?.toLowerCase().includes(term) ||
        user.email?.toLowerCase().includes(term) ||
        user.phone?.includes(term)
      );
    }

    // Apply ban status filter
    if (banFilter !== 'all') {
      filtered = filtered.filter(user => 
        banFilter === 'banned' ? user.isBanned : !user.isBanned
      );
    }

    return filtered;
  };

  const paginateUsers = (users, page) => {
    const startIndex = (page - 1) * ITEMS_PER_PAGE;
    return users.slice(startIndex, startIndex + ITEMS_PER_PAGE);
  };

  const filteredStudents = filterUsers(students);
  const filteredTeachers = filterUsers(teachers);
  const paginatedStudents = paginateUsers(filteredStudents, currentPage.students);
  const paginatedTeachers = paginateUsers(filteredTeachers, currentPage.teachers);

  // Clear all filters
  const clearFilters = () => {
    setSearchTerm('');
    setBanFilter('all');
    setCurrentPage({ students: 1, teachers: 1 });
  };

  // Check if any filter is active
  const hasActiveFilters = searchTerm || banFilter !== 'all';

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50/30">
      {/* Header */}
      <div className="bg-white border-b border-gray-200 shadow-sm">
        <div className="max-w-7xl mx-auto px-6 py-6">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900 bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
                Plan&Practice Admin Dashboard
              </h1>
              <p className="text-gray-600 mt-2">Manage students and teachers efficiently</p>
            </div>
            <button
              onClick={handleRefreshAll}
              className="flex items-center gap-2 px-4 py-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors shadow-sm"
            >
              <RefreshCw className="h-4 w-4" />
              Refresh All
            </button>
          </div>

          {/* Navigation Tabs */}
          <div className="mt-6 flex space-x-1 border-b border-gray-200">
            {['overview', 'students', 'teachers', 'analytics'].map((tab) => (
              <button
                key={tab}
                onClick={() => setActiveTab(tab)}
                className={`px-4 py-2 font-medium text-sm rounded-t-lg transition-colors ${
                  activeTab === tab
                    ? 'text-blue-600 border-b-2 border-blue-600 bg-blue-50/50'
                    : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100/50'
                }`}
              >
                {tab.charAt(0).toUpperCase() + tab.slice(1)}
              </button>
            ))}
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-6 py-8 space-y-8">
        {/* Stats Overview */}
        {activeTab === 'overview' && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-600">Total Students</p>
                  <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalStudents}</p>
                </div>
                <div className="p-3 bg-blue-100 rounded-xl">
                  <GraduationCap className="h-6 w-6 text-blue-600" />
                </div>
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-600">Total Teachers</p>
                  <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalTeachers}</p>
                </div>
                <div className="p-3 bg-purple-100 rounded-xl">
                  <BookOpen className="h-6 w-6 text-purple-600" />
                </div>
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-600">Active Users</p>
                  <p className="text-2xl font-bold text-green-600 mt-1">{stats.activeUsers}</p>
                </div>
                <div className="p-3 bg-green-100 rounded-xl">
                  <Users className="h-6 w-6 text-green-600" />
                </div>
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-600">Banned Users</p>
                  <p className="text-2xl font-bold text-red-600 mt-1">{stats.bannedUsers}</p>
                </div>
                <div className="p-3 bg-red-100 rounded-xl">
                  <Shield className="h-6 w-6 text-red-600" />
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Enhanced Search and Filter Bar */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <div className="flex flex-col lg:flex-row gap-4 items-start lg:items-center justify-between">
            <div className="flex flex-col sm:flex-row gap-4 flex-1 max-w-4xl">
              {/* Search Input */}
              <div className="relative flex-1 min-w-[300px]">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-5 w-5" />
                <input
                  type="text"
                  placeholder="Search by username, email, or phone..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent focus:outline-none transition-all"
                />
              </div>

              {/* Ban Status Filter */}
              <div className="flex gap-2 items-center">
                <select
                  value={banFilter}
                  onChange={(e) => setBanFilter(e.target.value)}
                  className="px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent focus:outline-none transition-all"
                >
                  <option value="all">All Users</option>
                  <option value="active">Active Only</option>
                  <option value="banned">Banned Only</option>
                </select>
              </div>
            </div>

            <div className="flex gap-3 w-full lg:w-auto">
              {/* Clear Filters Button */}
              {hasActiveFilters && (
                <button 
                  onClick={clearFilters}
                  className="flex items-center gap-2 px-4 py-3 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                >
                  Clear Filters
                </button>
              )}
              
              <button className="px-4 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium">
                Export Data
              </button>
            </div>
          </div>

          {/* Active Filters Display */}
          {hasActiveFilters && (
            <div className="mt-4 flex flex-wrap gap-2">
              {searchTerm && (
                <span className="inline-flex items-center gap-1 px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm">
                  Search: "{searchTerm}"
                  <button 
                    onClick={() => setSearchTerm('')}
                    className="hover:text-blue-600"
                  >
                    ×
                  </button>
                </span>
              )}
              {banFilter !== 'all' && (
                <span className="inline-flex items-center gap-1 px-3 py-1 bg-purple-100 text-purple-800 rounded-full text-sm">
                  Status: {banFilter === 'banned' ? 'Banned' : 'Active'}
                  <button 
                    onClick={() => setBanFilter('all')}
                    className="hover:text-purple-600"
                  >
                    ×
                  </button>
                </span>
              )}
            </div>
          )}

          {/* Results Count */}
          <div className="mt-4 text-sm text-gray-600">
            Showing {filteredStudents.length + filteredTeachers.length} users 
            ({filteredStudents.length} students, {filteredTeachers.length} teachers)
            {hasActiveFilters && ` (filtered from ${students.length + teachers.length} total)`}
          </div>
        </div>

        {/* Students Section */}
        {(activeTab === 'overview' || activeTab === 'students') && (
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 hover:shadow-md transition-shadow">
            <div className="p-6 border-b border-gray-200 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <GraduationCap className="h-6 w-6 text-blue-600" />
                </div>
                <div>
                  <h2 className="text-xl font-bold text-gray-900">Students Management</h2>
                  <p className="text-sm text-gray-600">
                    Showing {paginatedStudents.length} of {filteredStudents.length} students
                    {hasActiveFilters && ` (filtered from ${students.length} total)`}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <button
                  onClick={fetchStudents}
                  disabled={loading.students}
                  className="flex items-center gap-2 px-3 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 transition-colors"
                >
                  <RefreshCw className={`h-4 w-4 ${loading.students ? 'animate-spin' : ''}`} />
                  Refresh
                </button>
              </div>
            </div>

            <UserTable
              users={paginatedStudents}
              userType="Student"
              onBanToggle={handleBanToggle}
              onEdit={setEditingUser}
              loading={loading.students}
            />

            {filteredStudents.length > ITEMS_PER_PAGE && (
              <div className="p-6 border-t border-gray-200 bg-gray-50/50">
                <Pagination
                  currentPage={currentPage.students}
                  totalPages={Math.ceil(filteredStudents.length / ITEMS_PER_PAGE)}
                  onPageChange={(page) => setCurrentPage(prev => ({ ...prev, students: page }))}
                />
              </div>
            )}
          </div>
        )}

        {/* Teachers Section */}
        {(activeTab === 'overview' || activeTab === 'teachers') && (
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 hover:shadow-md transition-shadow">
            <div className="p-6 border-b border-gray-200 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-purple-100 rounded-lg">
                  <BookOpen className="h-6 w-6 text-purple-600" />
                </div>
                <div>
                  <h2 className="text-xl font-bold text-gray-900">Teachers Management</h2>
                  <p className="text-sm text-gray-600">
                    Showing {paginatedTeachers.length} of {filteredTeachers.length} teachers
                    {hasActiveFilters && ` (filtered from ${teachers.length} total)`}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <button
                  onClick={fetchTeachers}
                  disabled={loading.teachers}
                  className="flex items-center gap-2 px-3 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 transition-colors"
                >
                  <RefreshCw className={`h-4 w-4 ${loading.teachers ? 'animate-spin' : ''}`} />
                  Refresh
                </button>
              </div>
            </div>

            <UserTable
              users={paginatedTeachers}
              userType="Teacher"
              onBanToggle={handleBanToggle}
              onEdit={setEditingUser}
              loading={loading.teachers}
            />

            {filteredTeachers.length > ITEMS_PER_PAGE && (
              <div className="p-6 border-t border-gray-200 bg-gray-50/50">
                <Pagination
                  currentPage={currentPage.teachers}
                  totalPages={Math.ceil(filteredTeachers.length / ITEMS_PER_PAGE)}
                  onPageChange={(page) => setCurrentPage(prev => ({ ...prev, teachers: page }))}
                />
              </div>
            )}
          </div>
        )}

        {/* Analytics Placeholder */}
        {activeTab === 'analytics' && (
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8 text-center">
            <BarChart3 className="h-16 w-16 text-gray-400 mx-auto mb-4" />
            <h3 className="text-xl font-bold text-gray-900 mb-2">Analytics Dashboard</h3>
            <p className="text-gray-600 max-w-md mx-auto">
              User engagement metrics, platform usage statistics, and performance analytics will be displayed here.
            </p>
            <button className="mt-6 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium">
              Generate Reports
            </button>
          </div>
        )}
      </div>

      {/* Toast */}
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={hideToast}
        />
      )}
    </div>
  );
};

export default AdminDashboard;